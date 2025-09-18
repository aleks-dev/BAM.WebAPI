
using AutoMapper;
using BAM.DataAccessLayer.Repos;
using BAM.Domain.Enums;
using BAM.Domain.Models;
using BAM.Infra.Database;
using BAM.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BAM.DataAccessLayer.Tests.Repos
{
    public class AccountRepoTests
    {
        private BAMDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<BAMDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new BAMDbContext(options);
        }

        private AccountRepo CreateRepo(BAMDbContext ctx, Mock<IMapper>? mapperMock = null, Mock<ILogger<AccountRepo>>? loggerMock = null)
        {
            var mapper = mapperMock?.Object ?? Mock.Of<IMapper>();
            var logger = loggerMock?.Object ?? Mock.Of<ILogger<AccountRepo>>();
            return new AccountRepo(ctx, mapper, logger);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenNotFound()
        {
            var ctx = CreateContext(Guid.NewGuid().ToString());
            var mapperMock = new Mock<IMapper>();
            var repo = CreateRepo(ctx, mapperMock);

            var result = await repo.GetAsync(123);

            Assert.Null(result);
            mapperMock.Verify(m => m.Map<Account>(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task GetAsync_ReturnsMappedAccount_WhenFound()
        {
            var dbName = Guid.NewGuid().ToString();
            var ctx = CreateContext(dbName);

            var entity = new AccountEntity
            {
                Id = 1,
                BankId = 1,
                CustomerId = 1,
                BankAccountId = Guid.NewGuid(),
                Balance = 100m,
                AccountType = AccountType.Current
            };

            ctx.Accounts.Add(entity);
            await ctx.SaveChangesAsync();

            var mapperMock = new Mock<IMapper>();
            var mappedAccount = Mock.Of<Account>();
            mapperMock.Setup(m => m.Map<Account>(It.IsAny<object>()))
                      .Returns(mappedAccount);

            var repo = CreateRepo(ctx, mapperMock);

            var result = await repo.GetAsync(1);

            Assert.Same(mappedAccount, result);
            mapperMock.Verify(m => m.Map<Account>(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GetAllByCustomerId_ReturnsMappedList_WhenCustomerExists()
        {
            var dbName = Guid.NewGuid().ToString();
            var ctx = CreateContext(dbName);

            var acc1 = new AccountEntity
            {
                Id = 10,
                BankId = 1,
                CustomerId = 5,
                BankAccountId = Guid.NewGuid(),
                Balance = 50m,
                AccountType = AccountType.Savings
            };

            var customer = new CustomerEntity
            {
                Id = 5,
                Name = "Test Customer",
                CreditRating = 50,
                Accounts = new List<AccountEntity> { acc1 }
            };

            ctx.Customers.Add(customer);
            ctx.Accounts.Add(acc1);
            await ctx.SaveChangesAsync();

            var expectedList = new List<Account> { Mock.Of<Account>() };
            var mapperMock = new Mock<IMapper>();
            // Accept any source object and return our expected mapped list
            mapperMock.Setup(m => m.Map<IList<Account>>(It.IsAny<object>()))
                      .Returns(expectedList);

            var repo = CreateRepo(ctx, mapperMock);

            var result = await repo.GetAllByCustomerId(5);

            Assert.Same(expectedList, result);
            mapperMock.Verify(m => m.Map<IList<Account>>(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GetAllByCustomerId_Throws_WhenCustomerNotFound()
        {
            var ctx = CreateContext(Guid.NewGuid().ToString());
            var mapperMock = new Mock<IMapper>();
            var repo = CreateRepo(ctx, mapperMock);

            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.GetAllByCustomerId(999));
            mapperMock.Verify(m => m.Map<IList<Account>>(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task TransferMoneyBetweenAccountsAsync_Throws_ForNonPositiveAmount()
        {
            var ctx = CreateContext(Guid.NewGuid().ToString());
            var repo = CreateRepo(ctx);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repo.TransferMoneyBetweenAccountsAsync(1, 2, 0m));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repo.TransferMoneyBetweenAccountsAsync(1, 2, -5m));
        }

        [Fact]
        public async Task TransferMoneyBetweenAccountsAsync_Throws_WhenAccountNotFound()
        {
            var ctx = CreateContext(Guid.NewGuid().ToString());
            // no accounts seeded
            var repo = CreateRepo(ctx);

            await Assert.ThrowsAsync<ArgumentException>(() => repo.TransferMoneyBetweenAccountsAsync(1, 2, 10m));
        }

        [Fact]
        public async Task TransferMoneyBetweenAccountsAsync_Throws_WhenInsufficientFunds()
        {
            var dbName = Guid.NewGuid().ToString();
            var ctx = CreateContext(dbName);

            var from = new AccountEntity
            {
                Id = 1,
                BankId = 1,
                CustomerId = 1,
                BankAccountId = Guid.NewGuid(),
                Balance = 5m,
                AccountType = AccountType.Current
            };
            var to = new AccountEntity
            {
                Id = 2,
                BankId = 1,
                CustomerId = 2,
                BankAccountId = Guid.NewGuid(),
                Balance = 0m,
                AccountType = AccountType.Current
            };

            ctx.Accounts.AddRange(from, to);
            await ctx.SaveChangesAsync();

            var repo = CreateRepo(ctx);

            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.TransferMoneyBetweenAccountsAsync(1, 2, 10m));
        }

        [Fact]
        public async Task TransferMoneyBetweenAccountsAsync_Transfers_WhenSufficientFunds()
        {
            var dbName = Guid.NewGuid().ToString();
            var ctx = CreateContext(dbName);

            var from = new AccountEntity
            {
                Id = 100,
                BankId = 1,
                CustomerId = 1,
                BankAccountId = Guid.NewGuid(),
                Balance = 200m,
                AccountType = AccountType.Current
            };
            var to = new AccountEntity
            {
                Id = 200,
                BankId = 1,
                CustomerId = 2,
                BankAccountId = Guid.NewGuid(),
                Balance = 50m,
                AccountType = AccountType.Savings
            };

            ctx.Accounts.AddRange(from, to);
            await ctx.SaveChangesAsync();

            var repo = CreateRepo(ctx);

            await repo.TransferMoneyBetweenAccountsAsync(100, 200, 75m);

            // reload from DB to verify persisted changes
            var updatedFrom = await ctx.Accounts.FirstAsync(a => a.Id == 100);
            var updatedTo = await ctx.Accounts.FirstAsync(a => a.Id == 200);

            Assert.Equal(125m, updatedFrom.Balance);
            Assert.Equal(125m, updatedTo.Balance);
        }
    }
}

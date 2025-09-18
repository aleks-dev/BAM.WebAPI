using AutoMapper;
using BAM.DataAccessLayer.Repos;
using BAM.Infra.Database;
using BAM.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BAM.DataAccessLayer.Tests.Repos
{
    public class CustomerRepoTests
    {
        private static ILogger<CustomerRepo> CreateLogger() => NullLogger<CustomerRepo>.Instance;

        private static BAMDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<BAMDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new BAMDbContext(options);
        }

        private CustomerRepo CreateRepo(BAMDbContext ctx, Mock<IMapper>? mapperMock = null, Mock<ILogger<CustomerRepo>>? loggerMock = null)
        {
            var mapper = mapperMock?.Object ?? Mock.Of<IMapper>();
            var logger = loggerMock?.Object ?? Mock.Of<ILogger<CustomerRepo>>();
            return new CustomerRepo(ctx, mapper, logger);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenCustomerNotFound()
        {
            // Arrange
            await using var ctx = CreateContext(Guid.NewGuid().ToString());
            var mapper = new Mock<IMapper>();
            var logger = CreateLogger();
            var repo = CreateRepo(ctx, mapper);

            // Act
            var result = await repo.GetAsync(12345);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ReturnsMappedCustomer_WhenCustomerExists()
        {
            // Arrange
            await using var ctx = CreateContext(Guid.NewGuid().ToString());
            var existing = new CustomerEntity
            {
                Id = 1,
                Name = "Alice",
                CreditRating = 85
            };
            ctx.Customers.Add(existing);
            await ctx.SaveChangesAsync();

            var mapper = new Mock<IMapper>();
            var logger = CreateLogger();
            var repo = CreateRepo(ctx, mapper);

            // Act
            var result = await repo.GetAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existing.Id, result!.Id);
            Assert.Equal(existing.Name, result.Name);
            Assert.Equal(existing.CreditRating, result.CreditRating);
        }
    }
}

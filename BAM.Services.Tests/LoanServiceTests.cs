using BAM.Contracts.DTO;
using BAM.Contracts.Enums;
using BAM.DataAccessLayer.Interfaces;
using BAM.DataAccessLayer.UnitOfWork;
using BAM.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace BAM.Services.Tests.Services
{
    public class LoanServiceTests
    {
        private readonly Mock<IInterestService> _interestService = new(MockBehavior.Strict);
        private readonly Mock<IAccountRepo> _accountRepo = new(MockBehavior.Strict);
        private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);
        private readonly Mock<ILogger<LoanService>> _logger = new();

        private LoanService CreateSut() => new(_interestService.Object, _accountRepo.Object, _uow.Object, _logger.Object);

        private static LoanApplicationDto MakeValidApplication(int accountId = 10, int customerId = 1, decimal amount = 100m,
            LoanDuration duration = LoanDuration.ThreeYears) => new()
        {
            AccountId = accountId,
            CustomerId = customerId,
            Amount = amount,
            Duration = duration
        };

        [Fact]
        public async Task ApplyForLoanAsync_ReturnsTrue_OnHappyPath()
        {
            // Arrange
            var app = MakeValidApplication();
            var account = new Account();
            // In code, repo is queried by customerId but also checks Id == AccountId.
            // Provide account with matching Id.
            typeof(Account).GetProperty("Id")!.SetValue(account, app.AccountId);

            _accountRepo.Setup(r => r.GetByIdAsync(app.CustomerId))
                .ReturnsAsync(account);

            var expectedRate = 7.5m;
            _interestService.Setup(s => s.GetInterestRate(app.CustomerId, (int)app.Duration))
                .Returns(expectedRate);

            _uow.Setup(u => u.ApplyForLoanAsync(It.Is<Loan>(l => l.Amount == app.Amount && l.InterestRate == expectedRate && l.Account == account)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var sut = CreateSut();

            // Act
            var ok = await sut.ApplyForLoanAsync(app);

            // Assert
            Assert.True(ok);
            _accountRepo.Verify(r => r.GetByIdAsync(app.CustomerId), Times.Once);
            _interestService.Verify(s => s.GetInterestRate(app.CustomerId, (int)app.Duration), Times.Once);
            _uow.Verify();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(10001)]
        public async Task ApplyForLoanAsync_Throws_WhenAmountOutOfRange(decimal amount)
        {
            var app = MakeValidApplication(amount: (decimal)amount);
            var sut = CreateSut();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.ApplyForLoanAsync(app));
        }

        [Fact]
        public async Task ApplyForLoanAsync_Throws_WhenAccountNotFoundOrMismatch()
        {
            var app = MakeValidApplication();
            // account Id mismatch
            var account = new Account();
            typeof(Account).GetProperty("Id")!.SetValue(account, app.AccountId + 1);

            _accountRepo.Setup(r => r.GetByIdAsync(app.CustomerId)).ReturnsAsync(account);

            var sut = CreateSut();
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ApplyForLoanAsync(app));
        }

        [Fact]
        public async Task ApplyForLoanAsync_ReturnsFalse_WhenLowCreditRating_ExceptionMessageHandled()
        {
            var app = MakeValidApplication();
            var account = new Account();
            typeof(Account).GetProperty("Id")!.SetValue(account, app.AccountId);
            _accountRepo.Setup(r => r.GetByIdAsync(app.CustomerId)).ReturnsAsync(account);

            _interestService.Setup(s => s.GetInterestRate(app.CustomerId, (int)app.Duration))
                .Throws(new InvalidOperationException("Credit rating too low for loan"));

            var sut = CreateSut();

            var result = await sut.ApplyForLoanAsync(app);

            Assert.False(result);
            _uow.Verify(u => u.ApplyForLoanAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Fact]
        public async Task ApplyForLoanAsync_Rethrows_UnexpectedInvalidOperationException()
        {
            var app = MakeValidApplication();
            var account = new Account();
            typeof(Account).GetProperty("Id")!.SetValue(account, app.AccountId);
            _accountRepo.Setup(r => r.GetByIdAsync(app.CustomerId)).ReturnsAsync(account);

            _interestService.Setup(s => s.GetInterestRate(app.CustomerId, (int)app.Duration))
                .Throws(new InvalidOperationException("Other error"));

            var sut = CreateSut();
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ApplyForLoanAsync(app));
        }
    }
}

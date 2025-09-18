using BAM.DataAccessLayer.Interfaces;
using BAM.DataAccessLayer.UnitOfWork;
using BAM.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;


public class ApplyForLoanUnitOfWorkTests
{
    private readonly Mock<ILoanRepo> _loanRepoMock;
    private readonly Mock<ILogger<ApplyForLoanUnitOfWork>> _loggerMock;
    private readonly ApplyForLoanUnitOfWork _unitOfWork;

    public ApplyForLoanUnitOfWorkTests()
    {
        _loanRepoMock = new Mock<ILoanRepo>(MockBehavior.Strict);
        _loggerMock = new Mock<ILogger<ApplyForLoanUnitOfWork>>(MockBehavior.Strict);
        _unitOfWork = new ApplyForLoanUnitOfWork(_loanRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ApplyLoanTransactionAsync_Should_CallRepoMethodsAndComplete_When_Succeeds()
    {
        // Arrange
        int accountId = 123;
        decimal amount = 1000m;
        var loan = new Loan { AccountId = accountId, Id = 1, Amount = amount, InterestRate = 5m };

        _loanRepoMock.Setup(r => r.IncreaseAccountBalanceByAmountAsync(accountId, amount))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _loanRepoMock.Setup(r => r.AddAsync(loan))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // logger: allow any Log invocations (we'll verify specific ones)
        _loggerMock.Setup(l => l.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()))
            .Verifiable();

        // Act
        await _unitOfWork.ApplyLoanTransactionAsync(accountId, loan, amount);

        // Assert
        _loanRepoMock.Verify(r => r.IncreaseAccountBalanceByAmountAsync(accountId, amount), Times.Once);
        _loanRepoMock.Verify(r => r.AddAsync(loan), Times.Once);

        // Verify the completion informational log happened
        _loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("UnitOfWork transaction completed")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task ApplyLoanTransactionAsync_Should_ThrowArgumentNullException_When_LoanIsNull()
    {
        // Arrange
        int accountId = 1;
        Loan? loan = null;
        decimal amount = 100m;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _unitOfWork.ApplyLoanTransactionAsync(accountId, loan!, amount));
    }

    [Fact]
    public async Task ApplyLoanTransactionAsync_Should_ThrowArgumentOutOfRangeException_When_AmountNegative()
    {
        // Arrange
        int accountId = 1;
        var loan = new Loan { AccountId = accountId, Id = 1, Amount = 10m };
        decimal negativeAmount = -5m;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _unitOfWork.ApplyLoanTransactionAsync(accountId, loan, negativeAmount));
    }

    [Fact]
    public async Task ApplyLoanTransactionAsync_Should_Rethrow_When_IncreaseFails_And_NotCallAdd()
    {
        // Arrange
        int accountId = 42;
        decimal amount = 500m;
        var loan = new Loan { AccountId = accountId, Id = 2, Amount = amount };

        var ex = new InvalidOperationException("increase failed");
        _loanRepoMock.Setup(r => r.IncreaseAccountBalanceByAmountAsync(accountId, amount))
            .ThrowsAsync(ex)
            .Verifiable();

        // AddAsync should not be called; set up no expectation for it (strict mock will fail if called)
        // Logger should be called; allow log calls
        _loggerMock.Setup(l => l.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()))
            .Verifiable();

        // Act & Assert
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _unitOfWork.ApplyLoanTransactionAsync(accountId, loan, amount));

        Assert.Same(ex, thrown);

        _loanRepoMock.Verify(r => r.IncreaseAccountBalanceByAmountAsync(accountId, amount), Times.Once);
        _loanRepoMock.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Never);

        // Verify a warning about rollback was logged
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("UnitOfWork transaction failed")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ApplyLoanTransactionAsync_Should_Rethrow_When_AddFails_And_BalanceIncreased()
    {
        // Arrange
        int accountId = 77;
        decimal amount = 200m;
        var loan = new Loan { AccountId = accountId, Id = 3, Amount = amount };

        _loanRepoMock.Setup(r => r.IncreaseAccountBalanceByAmountAsync(accountId, amount))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var ex = new Exception("add failed");
        _loanRepoMock.Setup(r => r.AddAsync(loan))
            .ThrowsAsync(ex)
            .Verifiable();

        _loggerMock.Setup(l => l.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()))
            .Verifiable();

        // Act & Assert
        var thrown = await Assert.ThrowsAsync<Exception>(() =>
            _unitOfWork.ApplyLoanTransactionAsync(accountId, loan, amount));

        Assert.Same(ex, thrown);

        _loanRepoMock.Verify(r => r.IncreaseAccountBalanceByAmountAsync(accountId, amount), Times.Once);
        _loanRepoMock.Verify(r => r.AddAsync(loan), Times.Once);

        // Verify a warning about rollback was logged
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("UnitOfWork transaction failed")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
    }
}
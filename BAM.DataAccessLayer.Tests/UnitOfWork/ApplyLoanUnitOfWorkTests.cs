using BAM.Contracts.Enums;
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
    public async Task ApplyLoanTransactionAsync_Should_ThrowArgumentNullException_When_LoanIsNull()
    {
        // Arrange
        Loan? loan = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _unitOfWork.ApplyForLoanAsync(loan!));
    }

    [Fact]
    public async Task ApplyLoanTransactionAsync_Should_ThrowArgumentOutOfRangeException_When_AmountNegative()
    {
        // Arrange
        int accountId = 1;
        var account = new Account(accountId, AccountType.Loan);
        var loan = new Loan(account) { Id = 1, Amount = -10m };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _unitOfWork.ApplyForLoanAsync(loan));
    }
}
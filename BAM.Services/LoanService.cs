using BAM.Contracts.DTO;
using BAM.DataAccessLayer.Interfaces;
using BAM.DataAccessLayer.UnitOfWork;
using BAM.Domain.Models;
using BAM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BAM.Services
{    
    public class LoanService : ILoanService
    {
        private readonly IInterestService _interestService;
        private readonly IAccountRepo _accountRepo;
        private readonly ILogger<LoanService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public LoanService(IInterestService interestService, IAccountRepo accountRepo,
             IUnitOfWork unitOfWork, ILogger<LoanService> logger)
        {
            _interestService = interestService;
            _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> ApplyForLoanAsync(LoanApplicationDto loanApplication)
        {
            try
            {
                if (loanApplication.Amount <= 0 || loanApplication.Amount > Loan.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(loanApplication.Amount));

                var account = await _accountRepo.GetByIdAsync(loanApplication.CustomerId);
                if (account == null || account.Id != loanApplication.AccountId)
                    throw new InvalidOperationException($"Account not found for customer {loanApplication.CustomerId}");

                _logger.LogInformation("Getting interest rate for amount {amount} and duration {Duration}", 
                    loanApplication.Amount, (int)loanApplication.Duration);

                var interestRate = _interestService.GetInterestRate(loanApplication.CustomerId, (int)loanApplication.Duration);
                _logger.LogInformation("Obtained interest rate: {interestRate}%", interestRate);

                var loan = new Loan(account)
                {
                    Amount = loanApplication.Amount,
                    Duration = loanApplication.Duration,
                    InterestRate = interestRate
                };

                // Perform both repo operations transactionally via UnitOfWork
                await _unitOfWork.ApplyForLoanAsync(loan);

                return true;
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Credit rating too low for loan"))
                {
                    _logger.LogWarning("Loan application denied due to low credit rating for customer {CustomerId}", loanApplication.CustomerId);
                    return false;
                }
                throw;
            }
        }
    }
}

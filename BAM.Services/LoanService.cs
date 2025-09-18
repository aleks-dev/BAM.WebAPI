using AutoMapper;
using BAM.DataAccessLayer.Interfaces;
using BAM.DataAccessLayer.UnitOfWork;
using BAM.Domain.Enums;
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
        private readonly IMapper _mapper;

        public LoanService(IInterestService interestService, IAccountRepo accountRepo,
             IMapper mapper, IUnitOfWork unitOfWork, ILogger<LoanService> logger)
        {
            _interestService = interestService;
            _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> ApplyForLoanAsync(int customerId, int accountId, decimal amount, int durationYears)
        {
            try
            {
                if (amount <= 0 || amount > Loan.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(amount));

                var account = await _accountRepo.GetAsync(customerId);
                if (account == null || account.Id != accountId)
                    throw new InvalidOperationException($"Account not found for customer {customerId}");

                _logger.LogInformation("Getting interest rate for amount {amount} and duration {durationYears}", amount, durationYears);

                var interestRate = _interestService.GetInterestRate(customerId, durationYears);
                _logger.LogInformation("Obtained interest rate: {interestRate}%", interestRate);

                var loan = new Loan
                {
                    Amount = amount,
                    Duration = (LoanDuration)durationYears,
                    InterestRate = interestRate,
                    Account = _mapper.Map<Account>(account)
                };

                // Perform both repo operations transactionally via UnitOfWork
                await _unitOfWork.ApplyLoanTransactionAsync(accountId, loan, amount).ConfigureAwait(false);

                return true;
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Credit rating too low for loan"))
                {
                    _logger.LogWarning("Loan application denied due to low credit rating for customer {CustomerId}", customerId);
                    return false;
                }
                throw;
            }
        }
    }
}

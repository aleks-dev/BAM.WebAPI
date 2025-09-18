using BAM.Domain.Calculators;
using BAM.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BAM.Services
{
    public class InterestService : IInterestService
    {
        private readonly ILogger<InterestService> _logger;
        private readonly IInterestRateCalc _interestRateCalc;

        public InterestService(IInterestRateCalc interestRateCalc, ILogger<InterestService> logger)
        {
            _interestRateCalc = interestRateCalc;
            _logger = logger;
        }

        public decimal GetInterestRate(int creditRating, int durationYears)
        {
            if (creditRating < 20)
                throw new InvalidOperationException("Credit rating too low for loan");

            if (durationYears != (int)LoanDuration.OneYear || durationYears != (int)LoanDuration.ThreeYears || durationYears != (int)LoanDuration.FiveYears)
                throw new ArgumentOutOfRangeException(nameof(durationYears));

            _logger.LogInformation("Calculating interest rate for credit rating {CreditRating} and duration {DurationYears}", creditRating, durationYears);
            var rate = _interestRateCalc.CalculateInterestRate(creditRating, durationYears);
            _logger.LogInformation("Calculated interest rate: {Rate}%", rate);

            return rate;
        }
    }
}

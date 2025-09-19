namespace BAM.Domain.Calculators
{
    public interface IInterestRateCalc
    {
        decimal CalculateInterestRate(int creditRating, int durationYears);
    }

    /// <summary>
    /// Calculates interest rates using an extensible set of rules.
    /// Default rules are provided but additional rules can be injected via constructor.
    /// Rules are evaluated in order; the first matching rule supplies the rate.
    /// </summary>
    public class InterestRateCalc : IInterestRateCalc
    {
        private readonly IReadOnlyList<IInterestRateRule> _rules;

        public InterestRateCalc(IEnumerable<IInterestRateRule> rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));
            _rules = rules.ToList().AsReadOnly();
        }

        public decimal CalculateInterestRate(int creditRating, int durationYears)
        {
            if (durationYears <= 0) throw new ArgumentOutOfRangeException(nameof(durationYears), "Duration must be positive.");

            var match = _rules.FirstOrDefault(r => r.IsMatch(creditRating, durationYears));
            if (match == null)
                throw new InvalidOperationException($"No interest rate rule found for creditRating={creditRating}, durationYears={durationYears}.");

            return match.GetRate(creditRating, durationYears);
        }

    }

}

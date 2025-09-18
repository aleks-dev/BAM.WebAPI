namespace BAM.Domain.Calculators
{

    public interface IInterestRateRule
    {
        bool IsMatch(int creditRating, int durationYears);
        decimal GetRate(int creditRating, int durationYears);
    }

    public class RangeInterestRateRule : IInterestRateRule
    {
        private readonly int _minInclusive;
        private readonly int _maxExclusive;
        private readonly IReadOnlyDictionary<int, decimal> _ratesByDuration;

        public RangeInterestRateRule(int minInclusive, int maxExclusive, IDictionary<int, decimal> ratesByDuration)
        {
            if (ratesByDuration == null)
                throw new ArgumentNullException(nameof(ratesByDuration));
            if (minInclusive >= maxExclusive)
                throw new ArgumentException("minInclusive must be less than maxExclusive");

            _minInclusive = minInclusive;
            _maxExclusive = maxExclusive;
            _ratesByDuration = new Dictionary<int, decimal>(ratesByDuration);
        }

        public bool IsMatch(int creditRating, int durationYears) =>
            creditRating >= _minInclusive && creditRating < _maxExclusive && _ratesByDuration.ContainsKey(durationYears);

        public decimal GetRate(int creditRating, int durationYears)
        {
            if (!_ratesByDuration.TryGetValue(durationYears, out var rate))
                throw new InvalidOperationException($"No rate configured for duration {durationYears} in this rule.");

            return rate;
        }
    }

}

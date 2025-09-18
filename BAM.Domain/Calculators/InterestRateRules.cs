namespace BAM.Domain.Calculators
{
    public class InterestRateRules
    {
        public static IEnumerable<IInterestRateRule> GetDefaultRules()
        {
            yield return new RangeInterestRateRule(minInclusive: 20, maxExclusive: 50, new Dictionary<int, decimal>
            {
                [1] = 20m,
                [3] = 15m,
                [5] = 10m
            });

            yield return new RangeInterestRateRule(minInclusive: 50, maxExclusive: 101, new Dictionary<int, decimal>
            {
                [1] = 12m,
                [3] = 8m,
                [5] = 5m
            });
        }
    }
}

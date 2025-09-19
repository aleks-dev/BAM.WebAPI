namespace BAM.WebAPI.Options
{
    public sealed class InterestRateRulesOptions
    {
        public List<InterestRateRuleConfig> Rules { get; set; } = new();
    }

    public sealed class InterestRateRuleConfig
    {
        public int MinInclusive { get; set; }
        public int MaxExclusive { get; set; }
        public Dictionary<int, decimal> RatesByDuration { get; set; } = new();
    }
}

namespace BAM.Services
{
    public interface IInterestService
    {
        decimal GetInterestRate(int creditRating, int durationYears);
    }
}

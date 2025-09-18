namespace BAM.Services.Interfaces
{
    public interface ILoanService
    {
        Task<bool> ApplyForLoanAsync(int customerId, int accountId, decimal amount, int durationYears);
    }
}

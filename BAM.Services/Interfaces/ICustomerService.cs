using BAM.Contracts.DTO;

namespace BAM.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetByIdAsync(int id);
    }
}

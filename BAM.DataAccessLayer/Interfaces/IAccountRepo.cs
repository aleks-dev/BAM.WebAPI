using BAM.Domain.Models;

namespace BAM.DataAccessLayer.Interfaces
{
    public interface IAccountRepo : IRepo<Account>
    {
        Task<IList<Account>> GetAllByCustomerId(int customerId);
    }
}

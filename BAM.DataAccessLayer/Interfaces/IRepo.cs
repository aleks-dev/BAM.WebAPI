namespace BAM.DataAccessLayer.Interfaces
{
    public interface IRepo<T> where T : class
    {
        Task AddAsync(T obj);

        Task<T?> GetAsync(int id);

        Task<IList<T>> GetAllAsync();

        Task UpdateAsync(T obj);

        Task DeleteAsync(int id);
    }
}

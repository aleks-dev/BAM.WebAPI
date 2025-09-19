using AutoMapper;
using BAM.DataAccessLayer.Interfaces;
using BAM.Domain.Models;
using BAM.Infra.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BAM.DataAccessLayer.Repos
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly BAMDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerRepo> _logger;

        public CustomerRepo(BAMDbContext ctx, IMapper mapper, ILogger<CustomerRepo> logger)
        {
            _ctx = ctx; _mapper = mapper; _logger = logger;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            _logger.LogInformation("GetAsync called for id {Id}", id);
            var any = await _ctx.Customers.Where(u => u.Id == id).AnyAsync();
            if (!any)
            {
                _logger.LogInformation("GetAsync: no customer found for id {Id}", id);
                return null;
            }

            var entity = await _ctx.Customers.Where(u => u.Id == id).FirstAsync();
            _logger.LogInformation("GetAsync: found customer for id {Id}", id);
            return _mapper.Map<Customer>(entity);
        }

        #region Not Implemented
        // The following methods are not implemented as they are not needed for the current requirements.
        public async Task AddAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Customer>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
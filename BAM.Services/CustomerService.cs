using AutoMapper;
using BAM.Contracts.DTO;
using BAM.DataAccessLayer.Interfaces;
using BAM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BAM.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepo repo, IMapper mapper, ILogger<CustomerService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("CustomerService: GetAsync called");
            return _mapper.Map<CustomerDto>(await _repo.GetByIdAsync(id));

        }
    }
}

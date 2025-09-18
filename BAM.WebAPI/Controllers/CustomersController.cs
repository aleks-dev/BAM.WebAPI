using BAM.Contracts.DTO;
using BAM.DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BAM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepo _customerRepo;

        public CustomersController(ICustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }

        // GET api/customers
        [HttpGet]
        public async Task<ActionResult<IList<CustomerDto>>> GetAll()
        {
            var customers = await _customerRepo.GetAllAsync();
            var result = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                CreditRating = c.CreditRating
            }).ToList();

            return Ok(result);
        }

        // GET api/customers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> Get(int id)
        {
            var c = await _customerRepo.GetAsync(id);
            if (c == null) return NotFound();

            var dto = new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                CreditRating = c.CreditRating
            };

            return Ok(dto);
        }
    }
}

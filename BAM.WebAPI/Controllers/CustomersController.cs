using BAM.Contracts.DTO;
using BAM.Services;
using BAM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BAM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly IInterestService _interestService;

        public CustomersController(ICustomerService customerService, IAccountService accountService, IInterestService interestService)
        {
            _customerService = customerService;
            _accountService = accountService;
            _interestService = interestService;
        }

        // GET api/customers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> Get([FromRoute] int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null) 
                return NotFound("Customer Not Found");

            customer.Accounts = await _accountService.GetAccountsByCustomerIdAsync(id);
            
            return Ok(customer);
        }

        // GET api/customers/5/interestrate?durationYears=3
        [HttpGet("{id:int}/interestrate")]
        public async Task<ActionResult<decimal>> GetRate([FromRoute] int id, [FromQuery] int durationYears)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound("Customer Not Found");

            var rate = _interestService.GetInterestRate(customer.CreditRating, durationYears);
            return Ok(rate);
        }
    }
}

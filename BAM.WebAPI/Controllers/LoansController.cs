using BAM.Contracts.DTO;
using BAM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BAM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        // POST api/loans/apply
        [HttpPost("apply")]
        public async Task<ActionResult<bool>> Apply([FromQuery] int customerId, [FromQuery] int accountId, [FromQuery] decimal amount, [FromQuery] int durationYears)
        {
            var approved = await _loanService.ApplyForLoanAsync(customerId, accountId, amount, durationYears);
            return Ok(approved);
        }
    }
}

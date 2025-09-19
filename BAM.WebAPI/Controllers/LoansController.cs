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
        public async Task<ActionResult<bool>> Apply([FromBody] LoanApplicationDto loanApplication)
        {
            var approved = await _loanService.ApplyForLoanAsync(loanApplication);
            return Ok(approved);
        }
    }
}

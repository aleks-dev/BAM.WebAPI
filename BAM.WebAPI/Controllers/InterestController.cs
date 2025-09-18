using BAM.Services;
using Microsoft.AspNetCore.Mvc;

namespace BAM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterestController : ControllerBase
    {
        private readonly IInterestService _interestService;

        public InterestController(IInterestService interestService)
        {
            _interestService = interestService;
        }

        // GET api/interest/rate?creditRating=80&durationYears=3
        [HttpGet("rate")]
        public ActionResult<decimal> GetRate([FromQuery] int creditRating, [FromQuery] int durationYears)
        {
            var rate = _interestService.GetInterestRate(creditRating, durationYears);
            return Ok(rate);
        }
    }
}

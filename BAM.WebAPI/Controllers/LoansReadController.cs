using BAM.Contracts.DTO;
using BAM.DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BAM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansReadController : ControllerBase
    {
        private readonly ILoanRepo _loanRepo;

        public LoansReadController(ILoanRepo loanRepo)
        {
            _loanRepo = loanRepo;
        }

        // GET api/loansread
        [HttpGet]
        public async Task<ActionResult<IList<LoanDto>>> GetAll()
        {
            var loans = await _loanRepo.GetAllAsync();
            var result = loans.Select(l => new LoanDto
            {
                Id = l.Id,
                AccountId = l.Account.Id,
                BankId = l.Account.Bank.Id,
                BankAccountGuid = l.Account.BankAccountGuid,
                Balance = l.Account.Balance,
                AccountType = l.Account.AccountType,
                Amount = l.Amount,
                InterestRate = l.InterestRate,
                Duration = l.Duration.ToString()
            }).ToList();

            return Ok(result);
        }

        // GET api/loansread/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LoanDto>> Get(int id)
        {
            var l = await _loanRepo.GetAsync(id);
            if (l == null) return NotFound();

            var dto = new LoanDto
            {
                Id = l.Id,
                AccountId = l.Account.Id,
                BankId = l.Account.Bank.Id,
                BankAccountGuid = l.Account.BankAccountGuid,
                Balance = l.Account.Balance,
                AccountType = l.Account.AccountType,
                Amount = l.Amount,
                InterestRate = l.InterestRate,
                Duration = l.Duration.ToString()
            };

            return Ok(dto);
        }
    }
}

using BAM.Contracts.DTO;
using BAM.DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BAM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsReadController : ControllerBase
    {
        private readonly IAccountRepo _accountRepo;

        public AccountsReadController(IAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;
        }

        // GET api/accountsread
        [HttpGet]
        public async Task<ActionResult<IList<AccountDto>>> GetAll()
        {
            var accounts = await _accountRepo.GetAllAsync();
            var result = accounts.Select(a => new AccountDto
            {
                AccountId = a.Id,
                BankId = a.Bank.Id,
                BankAccountGuid = a.BankAccountGuid,
                Balance = a.Balance,
                AccountType = a.AccountType
            }).ToList();

            return Ok(result);
        }

        // GET api/accountsread/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccountDto>> Get(int id)
        {
            var a = await _accountRepo.GetAsync(id);
            if (a == null) return NotFound();

            var dto = new AccountDto
            {
                AccountId = a.Id,
                BankId = a.Bank.Id,
                BankAccountGuid = a.BankAccountGuid,
                Balance = a.Balance,
                AccountType = a.AccountType
            };

            return Ok(dto);
        }
    }
}

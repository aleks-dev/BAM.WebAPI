using BAM.Contracts.DTO;
using BAM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BAM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET api/accounts/customer/5
        [HttpGet("customer/{customerId:int}")]
        public async Task<ActionResult<IList<AccountDto>>> GetAccountsByCustomer(int customerId)
        {
            var accounts = await _accountService.GetAccountsByCustomerIdAsync(customerId);
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

        // POST api/accounts/transfer
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromQuery] int fromAccountId, [FromQuery] int toAccountId, [FromQuery] decimal amount)
        {
            await _accountService.TransferMoneyBetweenAccountsAsync(fromAccountId, toAccountId, amount);
            return NoContent();
        }
    }
}

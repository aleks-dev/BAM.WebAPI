using AutoMapper;
using BAM.Contracts.DTO;
using BAM.DataAccessLayer.Interfaces;
using BAM.DataAccessLayer.UnitOfWork;
using BAM.Domain.Models;
using BAM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BAM.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;
        private readonly ITransferBetweenAccountsUnitOfWork _transferBetweenAccountsUnitOfWork;
        public AccountService(IAccountRepo repo, ITransferBetweenAccountsUnitOfWork transferBetweenAccountsUnitOfWork,
            IMapper mapper, ILogger<AccountService> logger)
        {
            _repo = repo;
            _transferBetweenAccountsUnitOfWork = transferBetweenAccountsUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IList<AccountDto>> GetAccountsByCustomerIdAsync(int customerId)
        {
            _logger.LogInformation("AccountService: GetAccountsByCustomerIdAsync called for customerId {customerId}", customerId);
            return _mapper.Map<IList<AccountDto>>( await _repo.GetAllByCustomerId(customerId));
        }

        public async Task TransferMoneyBetweenAccountsAsync(TransferDto transfer)
        {
            _logger.LogInformation("AccountService: TransferMoneyBetweenAccountsAsync called for accounts: from {fromAccountId} to {toAccountId} and amount {Amount}",
                transfer.FromAccountId, transfer.ToAccountId, transfer.Amount);

            var fromAccount = _mapper.Map<Account>(await _repo.GetByIdAsync(transfer.FromAccountId));
            var toAccount = _mapper.Map<Account>(await _repo.GetByIdAsync(transfer.ToAccountId));

            await _transferBetweenAccountsUnitOfWork.TransferAsync(fromAccount, toAccount, transfer.Amount);
        }
    }
}

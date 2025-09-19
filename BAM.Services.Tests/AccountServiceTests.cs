using AutoMapper;
using BAM.Contracts.DTO;
using BAM.DataAccessLayer.Interfaces;
using BAM.DataAccessLayer.UnitOfWork;
using BAM.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace BAM.Services.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepo> _accountRepo = new(MockBehavior.Strict);
        private readonly Mock<ITransferBetweenAccountsUnitOfWork> _uow = new(MockBehavior.Strict);
        private readonly Mock<IMapper> _mapper = new(MockBehavior.Strict);
        private readonly Mock<ILogger<AccountService>> _logger = new();

        private AccountService CreateSut() => new(_accountRepo.Object, _uow.Object, _mapper.Object, _logger.Object);

        [Fact]
        public async Task GetAccountsByCustomerIdAsync_ReturnsMappedDtos()
        {
            // Arrange
            var customerId = 7;
            var accounts = new List<Account> { new Account(), new Account() };
            var accountDtos = new List<AccountDto> { new(), new() };

            _accountRepo.Setup(r => r.GetAllByCustomerId(customerId))
                .ReturnsAsync(accounts);
            _mapper.Setup(m => m.Map<IList<AccountDto>>(accounts))
                .Returns(accountDtos);

            var sut = CreateSut();

            // Act
            var result = await sut.GetAccountsByCustomerIdAsync(customerId);

            // Assert
            Assert.Same(accountDtos, result);
            _accountRepo.Verify(r => r.GetAllByCustomerId(customerId), Times.Once);
            _mapper.Verify(m => m.Map<IList<AccountDto>>(accounts), Times.Once);
        }

        [Fact]
        public async Task TransferMoneyBetweenAccountsAsync_LoadsAccountsAndDelegatesToUnitOfWork()
        {
            // Arrange
            var transfer = new TransferDto { FromAccountId = 1, ToAccountId = 2, Amount = 50m };

            var fromAccountModel = new Account();
            var toAccountModel = new Account();

            var fromAccountDto = new Account { };
            var toAccountDto = new Account { };

            _accountRepo.Setup(r => r.GetByIdAsync(transfer.FromAccountId))
                .ReturnsAsync(fromAccountModel);
            _accountRepo.Setup(r => r.GetByIdAsync(transfer.ToAccountId))
                .ReturnsAsync(toAccountModel);

            _mapper.Setup(m => m.Map<Account>(fromAccountModel))
                .Returns(fromAccountDto);
            _mapper.Setup(m => m.Map<Account>(toAccountModel))
                .Returns(toAccountDto);

            _uow.Setup(u => u.TransferAsync(fromAccountDto, toAccountDto, transfer.Amount))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var sut = CreateSut();

            // Act
            await sut.TransferMoneyBetweenAccountsAsync(transfer);

            // Assert
            _accountRepo.Verify(r => r.GetByIdAsync(transfer.FromAccountId), Times.Once);
            _accountRepo.Verify(r => r.GetByIdAsync(transfer.ToAccountId), Times.Once);
            _mapper.Verify(m => m.Map<Account>(fromAccountModel), Times.Once);
            _mapper.Verify(m => m.Map<Account>(toAccountModel), Times.Once);
            _uow.Verify(u => u.TransferAsync(fromAccountDto, toAccountDto, transfer.Amount), Times.Once);
        }
    }
}

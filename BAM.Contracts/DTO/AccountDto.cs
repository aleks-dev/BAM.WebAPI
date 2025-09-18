using BAM.Domain.Enums;

namespace BAM.Contracts.DTO
{
    public class AccountDto
    {
        public int AccountId { get; set; }  
        public int BankId { get; set; }
        public Guid BankAccountGuid { get; set; }
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
    }
}

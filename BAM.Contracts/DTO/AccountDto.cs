using BAM.Contracts.Enums;

namespace BAM.Contracts.DTO
{
    public class AccountDto
    {
        public int Id { get; set; }  
        public int BankId { get; set; }
        public Guid BankAccountGuid { get; set; }
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
    }
}

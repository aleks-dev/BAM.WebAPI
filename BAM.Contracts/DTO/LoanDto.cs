using BAM.Contracts.Enums;

namespace BAM.Contracts.DTO
{
    public class LoanDto 
    {
        public int Id { get; set; }
        public LoanDuration Duration { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Amount { get; set; }
        public static decimal MaxValue { get; } = 10000m;

        public AccountDto Account { get; set; } = new AccountDto();
    }
}
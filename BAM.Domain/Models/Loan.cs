using BAM.Contracts.Enums;

namespace BAM.Domain.Models
{
    public class Loan
    {
        public Loan(Account account)
        {
            Account = account;
        }

        public int Id { get; set; }

        public Account Account { get; set; }
        public LoanDuration Duration { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Amount { get; set; }

        public static decimal MaxValue { get; } = 10000;
    }
}

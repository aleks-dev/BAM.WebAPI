namespace BAM.Contracts.DTO
{
    public class LoanDto : AccountDto
    {
        public int Id { get; set; }
        public string Duration { get; set; } = string.Empty;
        public decimal InterestRate { get; set; }
        public decimal Amount { get; set; }
        public static decimal MaxValue { get; } = 10000m;
        public AccountDto? Account { get; set; }
    }
}

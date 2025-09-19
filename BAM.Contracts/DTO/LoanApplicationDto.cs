using BAM.Contracts.Enums;

namespace BAM.Contracts.DTO
{
    public class LoanApplicationDto
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public LoanDuration Duration { get; set; }
        public decimal Amount { get; set; }
    }
}

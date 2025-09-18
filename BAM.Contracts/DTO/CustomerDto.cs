namespace BAM.Contracts.DTO
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CreditRating { get; set; }

        public IList<LoanDto> Loans { get; set; } = new List<LoanDto>();
    }
}

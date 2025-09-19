using System.ComponentModel.DataAnnotations;

namespace BAM.Contracts.DTO
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Range(1, 100)]
        public int CreditRating { get; set; }

        public IList<AccountDto> Accounts { get; set; } = new List<AccountDto>();
    }
}

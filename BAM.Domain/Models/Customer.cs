using System.ComponentModel.DataAnnotations;

namespace BAM.Domain.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [Range(1, 100)]
        public int CreditRating { get; set; }
        
        public IList<Account> Accounts { get; set; } = new List<Account>();

        public IList<Loan>? Loans { get; set; }
    }
}

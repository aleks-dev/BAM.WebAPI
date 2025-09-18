using BAM.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAM.Infra.Entities
{
    [Table("Loan")]
    public class LoanEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AccountId { get; set; }

        public LoanDuration Duration { get; set; }

        [Range(0, 100)]
        public decimal InterestRate { get; set; }

        public decimal Amount { get; set; }
    }
}

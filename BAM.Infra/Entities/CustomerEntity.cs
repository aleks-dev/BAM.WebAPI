using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAM.Infra.Entities
{

    [Table("Customer")]
    public class CustomerEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100)]
        public int CreditRating { get; set; }

        public ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();

        public ICollection<LoanEntity>? Loans { get; set; }
    }
}
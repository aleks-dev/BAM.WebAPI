using BAM.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAM.Infra.Entities
{
    [Table("Account")]
    public class AccountEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int BankId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public Guid BankAccountId { get; set; }

        public decimal Balance { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        public BankEntity Bank { get; set; } = null!;
        public CustomerEntity Customer { get; set; } = null!;
        public LoanEntity? Loan { get; set; }
    }
}
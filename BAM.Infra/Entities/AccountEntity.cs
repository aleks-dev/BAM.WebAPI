using BAM.Contracts.Enums;
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

        public int BankId { get; set; }

        public int CustomerId { get; set; }

        public Guid BankAccountId { get; set; }

        public decimal Balance { get; set; }

        public AccountType AccountType { get; set; }
    }
}
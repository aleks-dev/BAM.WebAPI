using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAM.Infra.Entities
{

    [Table("Bank")]
    public class BankEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(16)]
        public string BIC { get; set; } = string.Empty;

        public ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
    }
}

using BAM.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BAM.Infra.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
    {
        public void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.ToTable("Account");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BankId).IsRequired();
            builder.Property(x => x.CustomerId).IsRequired();
            builder.HasIndex(x => new { x.BankId, x.CustomerId} )
                   .IsUnique();

            builder.Property(x => x.BankAccountId)
                   .IsRequired();
            builder.HasIndex(x => x.BankAccountId)
                   .IsUnique();

            builder.Property(x => x.Balance)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(x => x.AccountType)
                   .IsRequired();
        }
    }
}

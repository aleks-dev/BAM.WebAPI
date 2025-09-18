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

            builder.Property(x => x.BankAccountId)
                   .IsRequired();

            builder.HasIndex(x => x.BankAccountId)
                   .IsUnique();

            builder.Property(x => x.Balance)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(x => x.AccountType)
                   .IsRequired();

            builder.HasOne(a => a.Bank)
                   .WithMany(b => b.Accounts)
                   .HasForeignKey(a => a.BankId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Customer)
                   .WithMany(c => c.Accounts)
                   .HasForeignKey(a => a.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Loan);
        }
    }
}

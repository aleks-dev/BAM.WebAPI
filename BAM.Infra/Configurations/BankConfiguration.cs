using BAM.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BAM.Infra.Configurations
{
    public class BankConfiguration : IEntityTypeConfiguration<BankEntity>
    {
        public void Configure(EntityTypeBuilder<BankEntity> builder)
        {
            builder.ToTable("Bank");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.BIC)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasIndex(x => x.BIC)
                .IsUnique();

            builder.HasMany(b => b.Accounts)
                .WithOne(a => a.Bank!)
                .HasForeignKey(a => a.BankId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
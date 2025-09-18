using BAM.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BAM.Infra.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<LoanEntity>
    {
        public void Configure(EntityTypeBuilder<LoanEntity> builder)
        {
            builder.ToTable("Loan");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AccountId).IsRequired();

            builder.Property(x => x.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(x => x.InterestRate)
                   .HasColumnType("decimal(5,2)")
                   .IsRequired();

            builder.Property(x => x.Duration)
                   .IsRequired();
        }
    }
}

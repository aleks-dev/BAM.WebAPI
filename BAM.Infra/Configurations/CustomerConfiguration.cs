using BAM.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BAM.Infra.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<CustomerEntity>
    {
        public void Configure(EntityTypeBuilder<CustomerEntity> builder)
        {
            builder.ToTable("Customer");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.CreditRating)
                .IsRequired();

            builder.HasMany(c => c.Accounts)
                .WithOne(a => a.Customer!)
                .HasForeignKey(a => a.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
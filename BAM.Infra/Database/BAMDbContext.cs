using BAM.Domain.Enums;
using BAM.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BAM.Infra.Database
{
    public class BAMDbContext : DbContext
    {
        public BAMDbContext(DbContextOptions<BAMDbContext> options) : base(options) { }

        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<LoanEntity> Loans { get; set; }
        public DbSet<BankEntity> Banks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BAMDbContext).Assembly);

            modelBuilder.Seed();
        }
    }

    public static class SeedDataExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Banks
            modelBuilder.Entity<BankEntity>().HasData(
                new BankEntity { Id = 1, Name = "First Bank", BIC = "FBICUS33" },
                new BankEntity { Id = 2, Name = "Second Bank", BIC = "SBICGB2L" }
            );

            // Customers
            modelBuilder.Entity<CustomerEntity>().HasData(
                new CustomerEntity { Id = 1, Name = "Anne", CreditRating = 80 },
                new CustomerEntity { Id = 2, Name = "Bob", CreditRating = 15 },
                new CustomerEntity { Id = 3, Name = "Jim", CreditRating = 45 }
            );

            // Accounts
            modelBuilder.Entity<AccountEntity>().HasData(
                new AccountEntity
                {
                    Id = 1,
                    BankId = 1,
                    CustomerId = 1,
                    BankAccountId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Balance = 1500.50m,
                    AccountType = AccountType.Current
                },
                new AccountEntity
                {
                    Id = 2,
                    BankId = 1,
                    CustomerId = 1,
                    BankAccountId = new Guid("22222222-2222-2222-2222-222222222222"),
                    Balance = 3200.00m,
                    AccountType = AccountType.Savings
                },
                new AccountEntity
                {
                    Id = 3,
                    BankId = 2,
                    CustomerId = 2,
                    BankAccountId = new Guid("33333333-3333-3333-3333-333333333333"),
                    Balance = 250.75m,
                    AccountType = AccountType.Current
                }
            );

            // Loans
            modelBuilder.Entity<LoanEntity>().HasData(
                new LoanEntity
                {
                    Id = 1,
                    AccountId = 1,
                    Amount = 5000.00m,
                    Duration = LoanDuration.ThreeYears,
                    InterestRate = 4.50m
                },
                new LoanEntity
                {
                    Id = 2,
                    AccountId = 3,
                    Amount = 12000.00m,
                    Duration = LoanDuration.FiveYears,
                    InterestRate = 5.25m
                }
            );
        }
    }
}

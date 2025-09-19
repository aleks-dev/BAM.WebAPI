using BAM.Contracts.Enums;

namespace BAM.Domain.Models
{
   public class Account
    {
        public int Id { get; private set; }

        public int BankId { get; private set; }

        public Guid BankAccountGuid { get; init; } = Guid.NewGuid();

        public decimal Balance { get; private set; }

        public AccountType AccountType { get; private set; }

        public Account() { }

        public Account(int bankId, AccountType accountType, Guid? bankAccountGuid = null)
        {
            if (bankId <= 0) throw new ArgumentOutOfRangeException(nameof(bankId));
            BankId = bankId;
            AccountType = accountType;
            BankAccountGuid = bankAccountGuid ?? Guid.NewGuid();
            Balance = 0m;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount));
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount));
            if (Balance - amount < 0m) throw new InvalidOperationException("Insufficient funds.");
            Balance -= amount;
        }
    }
}

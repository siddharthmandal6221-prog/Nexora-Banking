using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Repositories.Interfaces
{
    public interface IBankAccountRepository
    {
        BankAccount GetBankAccountById(int bankAccountId);

        List<BankAccount> GetAllBankAccounts();

        List<BankAccount> GetBankAccountsByCustomerId(int customerId);

        void AddBankAccount(BankAccount bankAccount);

        void UpdateBankAccount(BankAccount bankAccount);

        void DeleteBankAccount(BankAccount bankAccount);
    }
}
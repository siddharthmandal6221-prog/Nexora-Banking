using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        BankAccount? GetBankAccountById(int bankAccountId);

        BankAccount? GetBankAccountByNumber(
            string accountNumber);

        void AddTransaction(Transaction transaction);

        void SaveChanges();
    }
}
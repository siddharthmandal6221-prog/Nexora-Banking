using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Transaction GetTransactionById(int transactionId);

        List<Transaction> GetAllTransactions();

        List<Transaction> GetTransactionsByBankAccountId(int bankAccountId);

        void AddTransaction(Transaction transaction);
    }
}
using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Services
{
    public interface ITransactionService
    {
        Transaction GetTransactionById(int transactionId);

        List<Transaction> GetAllTransactions();

        List<Transaction> GetTransactionsByBankAccountId(int bankAccountId);

        void AddTransaction(Transaction transaction);
    }
}
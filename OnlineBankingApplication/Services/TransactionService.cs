using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public Transaction GetTransactionById(int transactionId)
        {
            return _transactionRepository.GetTransactionById(transactionId);
        }

        public List<Transaction> GetAllTransactions()
        {
            return _transactionRepository.GetAllTransactions();
        }

        public List<Transaction> GetTransactionsByBankAccountId(int bankAccountId)
        {
            return _transactionRepository.GetTransactionsByBankAccountId(bankAccountId);
        }

        public void AddTransaction(Transaction transaction)
        {
            _transactionRepository.AddTransaction(transaction);
        }
    }
}
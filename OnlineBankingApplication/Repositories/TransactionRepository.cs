using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Transaction GetTransactionById(int transactionId)
        {
            return _context.Transactions
                .Include(t => t.FromBankAccount)
                .FirstOrDefault(t => t.TransactionId == transactionId);
        }

        public List<Transaction> GetAllTransactions()
        {
            return _context.Transactions
                .Include(t => t.FromBankAccount)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        public List<Transaction> GetTransactionsByBankAccountId(int bankAccountId)
        {
            return _context.Transactions
                .Where(t => t.FromBankAccountId == bankAccountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        public void AddTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
    }
}
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public BankAccount? GetBankAccountById(int bankAccountId)
        {
            return _context.BankAccounts
                .FirstOrDefault(a => a.BankAccountId == bankAccountId);
        }

        public BankAccount? GetBankAccountByNumber(string accountNumber)
        {
            return _context.BankAccounts
                .FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        public void AddTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
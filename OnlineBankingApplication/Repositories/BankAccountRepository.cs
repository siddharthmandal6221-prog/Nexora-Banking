using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public BankAccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public BankAccount GetBankAccountById(int bankAccountId)
        {
            return _context.BankAccounts
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.BankAccountId == bankAccountId);
        }

        public List<BankAccount> GetAllBankAccounts()
        {
            return _context.BankAccounts
                .Include(b => b.Customer)
                .ToList();
        }

        public List<BankAccount> GetBankAccountsByCustomerId(int customerId)
        {
            return _context.BankAccounts
                .Include(b => b.Customer)
                .Where(b => b.CustomerId == customerId)
                .ToList();
        }

        public void AddBankAccount(BankAccount bankAccount)
        {
            if (bankAccount.CreatedDate == default || bankAccount.CreatedDate.Year < 2000)
            {
                bankAccount.CreatedDate = DateTime.Now;
            }
            _context.BankAccounts.Add(bankAccount);
            _context.SaveChanges();
        }

        public void UpdateBankAccount(BankAccount bankAccount)
        {
            // Get the already tracked entity from the database
            var existingAccount = _context.BankAccounts
                .FirstOrDefault(b =>
                    b.BankAccountId == bankAccount.BankAccountId);

            if (existingAccount == null)
            {
                return;
            }

            // Update only the editable scalar properties
            existingAccount.AccountNumber = bankAccount.AccountNumber;
            existingAccount.AccountType = bankAccount.AccountType;
            existingAccount.Balance = bankAccount.Balance;
            existingAccount.IsActive = bankAccount.IsActive;

            // Keep the existing CustomerId
            // so ownership cannot accidentally change.

            _context.SaveChanges();
        }

        public void DeleteBankAccount(BankAccount bankAccount)
        {
            var existingAccount = _context.BankAccounts
                .FirstOrDefault(b =>
                    b.BankAccountId == bankAccount.BankAccountId);

            if (existingAccount == null)
            {
                return;
            }

            _context.BankAccounts.Remove(existingAccount);
            _context.SaveChanges();
        }
    }
}
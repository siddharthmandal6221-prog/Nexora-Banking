using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountService(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public BankAccount GetBankAccountById(int bankAccountId)
        {
            return _bankAccountRepository.GetBankAccountById(bankAccountId);
        }

        public List<BankAccount> GetAllBankAccounts()
        {
            return _bankAccountRepository.GetAllBankAccounts();
        }

        public List<BankAccount> GetBankAccountsByCustomerId(int customerId)
        {
            return _bankAccountRepository.GetBankAccountsByCustomerId(customerId);
        }

        public void AddBankAccount(BankAccount bankAccount)
        {
            _bankAccountRepository.AddBankAccount(bankAccount);
        }

        public void UpdateBankAccount(BankAccount bankAccount)
        {
            _bankAccountRepository.UpdateBankAccount(bankAccount);
        }

        public void DeleteBankAccount(BankAccount bankAccount)
        {
            _bankAccountRepository.DeleteBankAccount(bankAccount);
        }
    }
}
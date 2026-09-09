using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.ViewModels
{
    public class AccountStatementViewModel
    {
        public BankAccount? Account { get; set; }

        public List<Transaction> Transactions { get; set; } = new();

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal TotalCredits { get; set; }

        public decimal TotalDebits { get; set; }

        public decimal CurrentBalance { get; set; }

        public int TransactionCount { get; set; }
    }
}
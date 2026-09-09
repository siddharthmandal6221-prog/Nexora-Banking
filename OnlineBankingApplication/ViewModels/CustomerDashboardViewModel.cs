using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.ViewModels
{
    public class CustomerDashboardViewModel
    {
        // ==========================================
        // CUSTOMER
        // ==========================================

        public string CustomerName { get; set; } = "Customer";


        // ==========================================
        // ACCOUNT SUMMARY
        // ==========================================

        public decimal TotalBalance { get; set; }

        public int AccountCount { get; set; }

        public int ActiveAccountCount { get; set; }


        // ==========================================
        // ACTIVITY SUMMARY
        // ==========================================

        public int TransactionCount { get; set; }

        public int BeneficiaryCount { get; set; }


        // ==========================================
        // DASHBOARD DATA
        // ==========================================

        public List<BankAccount> Accounts { get; set; } = new();

        public List<Beneficiary> Beneficiaries { get; set; } = new();

        public List<Transaction> RecentTransactions { get; set; } = new();

        public List<Card> Cards { get; set; } = new();

        public List<Subscription> Subscriptions { get; set; } = new();


        // ==========================================
        // MONTHLY OVERVIEW
        // ==========================================

        public Dictionary<string, decimal> MonthlyActivity
        {
            get;
            set;
        } = new();
    }
}
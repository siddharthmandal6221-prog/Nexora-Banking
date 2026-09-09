using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalCustomers { get; set; }

        public int TotalBankAccounts { get; set; }

        public int TotalTransactions { get; set; }

        public decimal TotalTransactionAmount { get; set; }

        public int PendingChequeRequests { get; set; }

        public int TotalBeneficiaries { get; set; }

        public int TotalBillPayments { get; set; }

        public decimal TotalBillPaymentAmount { get; set; }

        public List<Transaction> RecentTransactions { get; set; } = new();

        public List<AuditLog> RecentAuditLogs { get; set; } = new();

        public List<ApplicationUser> Users { get; set; } = new();
    }
}
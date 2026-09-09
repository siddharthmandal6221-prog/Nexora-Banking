using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.ViewModels
{
    public class TransactionHistoryViewModel
    {
        public List<Transaction> Transactions { get; set; } = new();

        public string? TransactionType { get; set; }

        public string? Status { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal TotalOutgoing { get; set; }

        public decimal TotalIncoming { get; set; }

        public int TransactionCount { get; set; }
    }
}
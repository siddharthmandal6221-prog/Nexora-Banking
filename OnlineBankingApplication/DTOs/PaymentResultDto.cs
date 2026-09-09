namespace OnlineBankingApplication.DTOs
{
    public class PaymentResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public int TransactionId { get; set; }

        public string FromAccountNumber { get; set; }

        public string ToAccountNumber { get; set; }

        public decimal Amount { get; set; }

        public decimal RemainingBalance { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }

        [Required]
        public int FromBankAccountId { get; set; }

        [Required]
        public string ToAccountNumber { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public string TransactionType { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Status { get; set; } = "Success";
    }
}
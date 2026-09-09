using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class PaymentDto
    {
        [Required]
        public int FromBankAccountId { get; set; }

        [Required]
        [StringLength(20)]
        public string ToAccountNumber { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(30)]
        public string TransactionType { get; set; } = "Transfer";
    }
}
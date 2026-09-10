using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class PaymentDto
    {
        [Required(ErrorMessage = "Source account is required.")]
        public int FromBankAccountId { get; set; }

        [Required(ErrorMessage = "Destination account number is required.")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "Account number must be between 9 and 20 characters.")]
        [RegularExpression(@"^[0-9]{9,20}$", ErrorMessage = "Destination account number must be between 9 and 20 digits.")]
        public string ToAccountNumber { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, 10000000, ErrorMessage = "Amount must be between 1 and 10,000,000.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Transaction type is required.")]
        [StringLength(30)]
        public string TransactionType { get; set; } = "Transfer";
    }
}
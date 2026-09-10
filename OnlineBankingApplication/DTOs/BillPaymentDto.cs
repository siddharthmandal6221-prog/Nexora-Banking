using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class BillPaymentDto
    {
        public int BillPaymentId { get; set; }

        [Required(ErrorMessage = "Bill type is required.")]
        [StringLength(50)]
        public string BillType { get; set; }

        [Required(ErrorMessage = "Biller name is required.")]
        [StringLength(100)]
        public string BillerName { get; set; }

        [Required(ErrorMessage = "Consumer number is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Consumer number must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "Consumer number can only contain letters, numbers, and hyphens.")]
        public string ConsumerNumber { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, 10000000, ErrorMessage = "Amount must be between 1 and 10,000,000.")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Status { get; set; } = "Success";

        [Required(ErrorMessage = "Bank account is required.")]
        public int BankAccountId { get; set; }

        public string? BankAccountNumber { get; set; }
    }
}
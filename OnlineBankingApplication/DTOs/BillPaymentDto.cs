using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class BillPaymentDto
    {
        public int BillPaymentId { get; set; }

        [Required]
        [StringLength(50)]
        public string BillType { get; set; }

        [Required]
        [StringLength(100)]
        public string BillerName { get; set; }

        [Required]
        [StringLength(50)]
        public string ConsumerNumber { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Status { get; set; } = "Success";

        [Required]
        public int BankAccountId { get; set; }

        public string? BankAccountNumber { get; set; }
    }
}
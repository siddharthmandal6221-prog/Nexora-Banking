using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class SubscriptionDto
    {
        public int SubscriptionId { get; set; }

        [Required]
        [StringLength(100)]
        public string MerchantName { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(30)]
        public string BillingCycle { get; set; } = "Monthly";

        public DateTime NextPaymentDate { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedDate { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        [Required]
        public int BankAccountId { get; set; }

        public string? BankAccountNumber { get; set; }
    }
}
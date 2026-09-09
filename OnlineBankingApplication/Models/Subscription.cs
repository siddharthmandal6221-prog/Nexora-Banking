using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBankingApplication.Models
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }

        [Required]
        [StringLength(100)]
        public string MerchantName { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(30)]
        public string BillingCycle { get; set; } = "Monthly";

        public DateTime NextPaymentDate { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public int BankAccountId { get; set; }

        [ForeignKey("BankAccountId")]
        public BankAccount? BankAccount { get; set; }
    }
}
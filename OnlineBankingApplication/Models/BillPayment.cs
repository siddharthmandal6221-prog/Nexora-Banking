using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBankingApplication.Models
{
    public class BillPayment
    {
        [Key]
        public int BillPaymentId { get; set; }

        [Required]
        public string BillType { get; set; }

        [Required]
        public string BillerName { get; set; }

        [Required]
        public string ConsumerNumber { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Success";

        public int BankAccountId { get; set; }

        [ForeignKey("BankAccountId")]
        public BankAccount? BankAccount { get; set; }
    }
}
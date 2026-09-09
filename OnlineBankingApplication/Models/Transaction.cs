using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBankingApplication.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int FromBankAccountId { get; set; }

        [Required]
        public string ToAccountNumber { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public string TransactionType { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Success";

        [ForeignKey("FromBankAccountId")]
        public BankAccount? FromBankAccount { get; set; }
    }
}
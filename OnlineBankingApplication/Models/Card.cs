using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBankingApplication.Models
{
    public class Card
    {
        [Key]
        public int CardId { get; set; }

        [Required]
        [StringLength(4)]
        public string LastFourDigits { get; set; }

        [Required]
        [StringLength(20)]
        public string CardNetwork { get; set; } = "VISA";

        [Required]
        [StringLength(50)]
        public string CardType { get; set; } = "Debit Card";

        [Required]
        [StringLength(100)]
        public string CardHolderName { get; set; }

        [Range(1, 12)]
        public int ExpiryMonth { get; set; }

        [Range(2026, 2100)]
        public int ExpiryYear { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int BankAccountId { get; set; }

        [ForeignKey("BankAccountId")]
        public BankAccount? BankAccount { get; set; }
    }
}
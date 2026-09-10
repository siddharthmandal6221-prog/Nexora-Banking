using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class CardDto
    {
        public int CardId { get; set; }

        [Required(ErrorMessage = "Last 4 digits are required.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Last four digits must be exactly 4 digits.")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Last four digits must be exactly 4 numeric digits.")]
        public string LastFourDigits { get; set; }

        [Required(ErrorMessage = "Card network is required.")]
        [StringLength(20)]
        public string CardNetwork { get; set; } = "VISA";

        [Required(ErrorMessage = "Card type is required.")]
        [StringLength(50)]
        public string CardType { get; set; } = "Debit Card";

        [Required(ErrorMessage = "Card holder name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Card holder name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Card holder name can only contain letters and spaces.")]
        public string CardHolderName { get; set; }

        [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
        public int ExpiryMonth { get; set; }

        [Range(2025, 2100, ErrorMessage = "Expiry year must be between 2025 and 2100.")]
        public int ExpiryYear { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        [Required]
        public int BankAccountId { get; set; }

        public string? BankAccountNumber { get; set; }
    }
}
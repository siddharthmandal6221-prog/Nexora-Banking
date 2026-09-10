using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class BankAccountDto
    {
        public int BankAccountId { get; set; }

        [Required(ErrorMessage = "Account number is required.")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "Account number must be between 9 and 20 characters.")]
        [RegularExpression(@"^[0-9]{9,20}$", ErrorMessage = "Account number must be between 9 and 20 digits.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Account type is required.")]
        public string AccountType { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Balance must be non-negative and within allowable limits.")]
        public decimal Balance { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        public int CustomerId { get; set; }
    }
}
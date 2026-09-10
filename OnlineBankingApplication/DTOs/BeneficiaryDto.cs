using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class BeneficiaryDto
    {
        public int BeneficiaryId { get; set; }

        [Required(ErrorMessage = "Beneficiary name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Beneficiary name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Beneficiary name can only contain letters and spaces.")]
        public string BeneficiaryName { get; set; }

        [Required(ErrorMessage = "Account number is required.")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "Account number must be between 9 and 20 characters.")]
        [RegularExpression(@"^[0-9]{9,20}$", ErrorMessage = "Account number must be between 9 and 20 digits.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Bank name is required.")]
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters.")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        public int CustomerId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class ChequeBookRequestDto
    {
        public int ChequeBookRequestId { get; set; }

        [Required(ErrorMessage = "Bank account is required.")]
        public int BankAccountId { get; set; }

        [Required(ErrorMessage = "Number of leaves is required.")]
        [Range(1, 100, ErrorMessage = "Number of leaves must be between 1 and 100.")]
        public int NumberOfLeaves { get; set; }

        public DateTime RequestDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string? BankAccountNumber { get; set; }

        public string? BankAccountType { get; set; }
    }
}
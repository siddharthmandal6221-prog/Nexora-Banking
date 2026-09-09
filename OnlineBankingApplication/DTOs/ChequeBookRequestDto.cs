using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class ChequeBookRequestDto
    {
        public int ChequeBookRequestId { get; set; }

        [Required]
        public int BankAccountId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of leaves must be greater than zero.")]
        public int NumberOfLeaves { get; set; }

        public DateTime RequestDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string? BankAccountNumber { get; set; }

        public string? BankAccountType { get; set; }
    }
}
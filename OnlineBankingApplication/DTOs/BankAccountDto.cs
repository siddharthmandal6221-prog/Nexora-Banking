using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class BankAccountDto
    {
        public int BankAccountId { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }

        [Required]
        public string AccountType { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        public int CustomerId { get; set; }
    }
}
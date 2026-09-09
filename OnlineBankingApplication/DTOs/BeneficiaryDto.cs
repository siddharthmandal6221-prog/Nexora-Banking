using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class BeneficiaryDto
    {
        public int BeneficiaryId { get; set; }

        [Required]
        [StringLength(100)]
        public string BeneficiaryName { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        public int CustomerId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBankingApplication.Models
{
    public class Beneficiary
    {
        [Key]
        public int BeneficiaryId { get; set; }

        [Required]
        [StringLength(100)]
        public string BeneficiaryName { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }

        [Required]
        public string BankName { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
    }
}
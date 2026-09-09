using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBankingApplication.Models
{
    public class ChequeBookRequest
    {
        [Key]
        public int ChequeBookRequestId { get; set; }

        [Required]
        public int NumberOfLeaves { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        public int BankAccountId { get; set; }

        [ForeignKey("BankAccountId")]
        public BankAccount? BankAccount { get; set; }
    }
}
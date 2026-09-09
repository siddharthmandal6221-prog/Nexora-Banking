using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        public string UserId { get; set; }

        [Required]
        public string Action { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
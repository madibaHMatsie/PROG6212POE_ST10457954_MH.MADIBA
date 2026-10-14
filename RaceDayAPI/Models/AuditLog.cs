using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action {  get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [MaxLength(4000)]
        public string? Details { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;


    }
}

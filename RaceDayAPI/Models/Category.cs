using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal EntryFee { get; set; }

        [MaxLength(50)]
        public string? AgeGroup { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Distance { get; set; }

        // Navigation Property — MUST reference "Event" (singular), not "Events"
        [ForeignKey("EventId")]
        public Events Events { get; set; } = null!;   // ← Fixed

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}
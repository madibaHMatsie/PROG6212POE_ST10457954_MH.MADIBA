using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrolmentId { get; set; }

        [Required]
        public int ParticipantId { get; set; }     

        [Required]
        public int EventId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Confirmed";

        public int? BibNumber { get; set; }

     
        [ForeignKey("ParticipantId")]
        public User Participant { get; set; } = null!;

        [ForeignKey("EventId")]
        public Events Events { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        public Result? Result { get; set; }
    }
}
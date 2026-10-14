using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Result
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        public TimeSpan? FinishTime { get; set; }
        public int? FinishPosition { get; set; }
        public int? CategoryPosition { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = " Pending";

        [ForeignKey("EnrolmentId")]
        public Enrolment Enrolment { get; set; } = null!;
       

    }
}

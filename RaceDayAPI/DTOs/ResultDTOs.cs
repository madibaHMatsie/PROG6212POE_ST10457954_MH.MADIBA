using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class ResultDto
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? FinishPosition { get; set; }
        public int? CategoryPosition { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ParticipantName { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CreateResultDto
    {
        [Required]
        public int EnrolmentId { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int FinishPosition { get; set; }

        [Range(1, int.MaxValue)]
        public int? CategoryPosition { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Official";
    }

    public class UpdateResultDto
    {
        public TimeSpan FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int FinishPosition { get; set; }

        [Range(1, int.MaxValue)]
        public int? CategoryPosition { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;
    }

    public class ParticipantResultDto
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? FinishPosition { get; set; }
        public int? CategoryPosition { get; set; }
        public string Status { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string EventLocation { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}

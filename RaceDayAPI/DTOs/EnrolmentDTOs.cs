using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class EnrolmentDto
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? BibNumber { get; set; }
        public ResultDto? Results { get; set; }
    }

    public class CreateEnrolmentDto
    {
        [Required]
        public int CategoryId { get; set; }
    }

    public class EnrolmentSummaryDto
    {
        public int TotalEnrolments { get; set; }
        public int ConfirmedCount { get; set; }
        public int PendingCount { get; set; }
        public int CancelledCount { get; set; }
        public List<EnrolmentDto> Enrolments { get; set; } = new List<EnrolmentDto>();
    }
}
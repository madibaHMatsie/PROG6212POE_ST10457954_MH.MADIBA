using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class EventDto
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public string OrganiserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public int CurrentEnrolments { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }

    public class CreateEventDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 999.99)]
        public decimal Distance { get; set; }

        [Required]
        [MaxLength(20)]
        public string EventType { get; set; } = string.Empty; // "run", "walk", "cycle"

        [Required]
        [Range(1, 100000)]
        public int MaxParticipants { get; set; }
    }

    public class UpdateEventDto
    {
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? Description { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;

        [Range(0.1, 999.99)]
        public decimal Distance { get; set; }

        [MaxLength(20)]
        public string EventType { get; set; } = string.Empty;

        [Range(1, 100000)]
        public int MaxParticipants { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // "Open", "Closed", "Cancelled"
    }
}
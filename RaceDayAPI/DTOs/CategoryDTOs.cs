using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal EntryFee { get; set; }
        public string? AgeGroup { get; set; }
        public decimal? Distance { get; set; }
        public int EnrolmentCount { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        [Range(0, 9999.99)]
        public decimal EntryFee { get; set; }

        [MaxLength(50)]
        public string? AgeGroup { get; set; }

        [Range(0.1, 999.99)]
        public decimal? Distance { get; set; }
    }

    public class UpdateCategoryDto
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        [Range(0, 9999.99)]
        public decimal EntryFee { get; set; }

        [MaxLength(50)]
        public string? AgeGroup { get; set; }

        [Range(0.1, 999.99)]
        public decimal? Distance { get; set; }
    }
}
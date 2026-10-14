using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class WeatherInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int WeatherInfoId { get; set; }
        [Required]
        public int EventId { get; set; }

        [Required]
        public DateTime ForecastDate { get; set; }
        public double? Temperature {  get; set; }

        [MaxLength(100)]
        public string? Condition { get; set; }
        public double? WindSpeed { get; set; }
        public double? Humidity { get; set; }

        [ForeignKey("EventId")]
        public Events Events { get; set; } = null!;

    }
}

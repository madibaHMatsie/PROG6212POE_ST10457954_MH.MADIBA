namespace RaceDayMVC.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public decimal Distance { get; set; }
        public string EventType { get; set; }
        public string Status { get; set; }
        public string? BannerImageUrl { get; set; } 
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal EntryFee { get; set; }
    }
}
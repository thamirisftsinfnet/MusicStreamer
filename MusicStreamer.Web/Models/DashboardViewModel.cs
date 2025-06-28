namespace MusicStreamer.Web.Models
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string SubscriptionPlan { get; set; } = "";
        public bool IsSubscriptionActive { get; set; }
        public List<MusicDto> FavoriteMusics { get; set; } = new();
        public List<BandDto> FavoriteBands { get; set; } = new();
    }

    public class MusicDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string BandName { get; set; } = "";
        public string AlbumTitle { get; set; } = "";
    }

    public class BandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}

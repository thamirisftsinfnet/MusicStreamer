namespace MusicStreamer.Web.Models
{
    public class MusicDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string AlbumTitle { get; set; } = string.Empty;
        public string AlbumCoverUrl { get; set; } = string.Empty;
        public string BandName { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
    }
}

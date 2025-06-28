namespace MusicStreamer.Web.Models
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; } = "";
        public List<MusicResult> Musics { get; set; } = new();
        public List<BandResult> Bands { get; set; } = new();
        public bool ResultsFound => Musics.Any() || Bands.Any();
    }

    public class MusicResult
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string BandName { get; set; } = "";
        public string AlbumTitle { get; set; } = "";
        public bool IsFavorite { get; set; }
    }

    public class BandResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsFavorite { get; set; }
    }
}

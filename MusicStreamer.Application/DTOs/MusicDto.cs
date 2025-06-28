using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.DTOs
{
    public class MusicDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public string FileUrl { get; set; }
        public int TrackNumber { get; set; }
        public AlbumDto Album { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class MusicSearchDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string BandName { get; set; }
        public string AlbumTitle { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsFavorite { get; set; }
    }
}

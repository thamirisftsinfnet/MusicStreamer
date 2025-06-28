using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.DTOs
{
    public class BandDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFavorite { get; set; }
        public List<AlbumDto> Albums { get; set; } = new List<AlbumDto>();
    }

    public class BandSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsFavorite { get; set; }
        public int AlbumsCount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.DTOs
{
    public class AlbumDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverImageUrl { get; set; }
        public BandDto Band { get; set; }
        public List<MusicDto> Musics { get; set; } = new List<MusicDto>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Entities
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverImageUrl { get; set; }
        public int BandId { get; set; }

        public virtual Band Band { get; set; }
        public virtual ICollection<Music> Musics { get; set; } = new List<Music>();
    }
}

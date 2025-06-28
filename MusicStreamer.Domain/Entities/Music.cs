using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Entities
{
    public class Music
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public string FileUrl { get; set; }
        public int TrackNumber { get; set; }
        public int AlbumId { get; set; }

        public virtual Album Album { get; set; }
        public virtual ICollection<PlaylistMusic> PlaylistMusics { get; set; } = new List<PlaylistMusic>();
        public virtual ICollection<UserFavoriteMusic> UserFavorites { get; set; } = new List<UserFavoriteMusic>();
    }
}

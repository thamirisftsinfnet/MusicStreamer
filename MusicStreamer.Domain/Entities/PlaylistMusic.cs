using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Entities
{
    public class PlaylistMusic
    {
        public int PlaylistId { get; set; }
        public int MusicId { get; set; }
        public int Order { get; set; }
        public DateTime AddedAt { get; set; }

        public virtual Playlist Playlist { get; set; }
        public virtual Music Music { get; set; }
    }
}

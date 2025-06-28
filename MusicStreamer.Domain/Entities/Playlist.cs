using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Entities
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<PlaylistMusic> PlaylistMusics { get; set; } = new List<PlaylistMusic>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Entities
{
    public class Band
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
        public virtual ICollection<UserFavoriteBand> UserFavorites { get; set; } = new List<UserFavoriteBand>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Entities
{
    public class UserFavoriteBand
    {
        public int UserId { get; set; }
        public int BandId { get; set; }
        public DateTime FavoritedAt { get; set; }

        public virtual User User { get; set; }
        public virtual Band Band { get; set; }
    }
}

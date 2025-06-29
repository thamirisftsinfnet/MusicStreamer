using MusicStreamer.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MusicStreamer.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public virtual Subscription Subscription { get; set; }
        public virtual ICollection<Playlist> Playlists { get; set; }
        public virtual ICollection<UserFavoriteMusic> FavoriteMusics { get; set; }
        public virtual ICollection<UserFavoriteBand> FavoriteBands { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<CreditCard> CreditCards { get; set; } 
    }
}

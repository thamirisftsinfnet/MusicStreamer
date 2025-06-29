using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.ValueObjects
{
    public class CreditCard
    {
        public int Id { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public string NumberMasked { get; set; } = string.Empty;
        public string Expiration { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; }
    }
}

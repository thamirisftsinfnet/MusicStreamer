using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.DTOs
{
    public class CreditCardDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public string NumberMasked { get; set; } = string.Empty;
        public string Expiration { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}

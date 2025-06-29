namespace MusicStreamer.Web.Models
{
    public class CreditCardViewModel
    {
        public string CardHolderName { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty; 
        public string Expiration { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
    }
    public class CreditCardDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CardHolderName { get; set; }
        public string NumberMasked { get; set; }
        public string Expiration { get; set; }
        public string Brand { get; set; }
        public string Token { get; set; }
    }
}

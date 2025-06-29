namespace MusicStreamer.Web.Models
{
    public class TransactionViewModel
    {
        public int PlanId { get; set; }  // plano selecionado
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class TransactionAuthorizeViewModel
    {
        public int TransactionId { get; set; }
        public int? SelectedCardId { get; set; }
        public List<CreditCardDto> AvailableCards { get; set; } = new();
    }

    public class TransactionListItemViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAuthorized => Status == "Authorized";
    }

    public class TransactionHistoryViewModel
    {
        public List<TransactionListItemViewModel> Pending { get; set; } = new();
        public List<TransactionListItemViewModel> Others { get; set; } = new();
    }

    public class CreditCard
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

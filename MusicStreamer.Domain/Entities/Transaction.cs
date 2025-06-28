using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string MerchantName { get; set; }
        public DateTime CreatedAt { get; set; }
        public TransactionStatus Status { get; set; }
        public string AuthorizationCode { get; set; }
        public string Description { get; set; }
        public virtual User User { get; set; }
    }

    public enum TransactionStatus
    {
        Pending = 0,
        Authorized = 1,
        Denied = 2,
        Cancelled = 3
    }
}

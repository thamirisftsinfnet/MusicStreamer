using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public SubscriptionPlanType PlanType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public decimal MonthlyFee { get; set; }

        public virtual User User { get; set; }
    }
    public enum SubscriptionPlanType
    {
        Free = 0,
        Premium = 1,
        Family = 2
    }
}

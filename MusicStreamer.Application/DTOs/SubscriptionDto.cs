using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public SubscriptionPlanType PlanType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public decimal MonthlyFee { get; set; }
    }

    public class CreateSubscriptionDto
    {
        public int UserId { get; set; }
        public SubscriptionPlanType PlanType { get; set; }
    }
}

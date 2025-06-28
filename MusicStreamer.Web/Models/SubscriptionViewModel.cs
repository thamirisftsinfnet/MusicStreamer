namespace MusicStreamer.Web.Models
{
    public class SubscriptionViewModel
    {
        public string CurrentPlan { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string NewPlan { get; set; } = string.Empty;

        public CreateSubscriptionPlanDto ToPlanDto(int userId)
        {
            return new CreateSubscriptionPlanDto
            {
                UserId = userId,
                PlanType = Enum.TryParse<SubscriptionPlanType>(NewPlan, out var parsed) ? parsed : SubscriptionPlanType.Free
            };
        }
    }

    public class CreateSubscriptionPlanDto
    {
        public int UserId { get; set; }
        public SubscriptionPlanType PlanType { get; set; }
    }

    public enum SubscriptionPlanType
    {
        Free = 0,
        Premium = 1,
        Family = 2
    }
}

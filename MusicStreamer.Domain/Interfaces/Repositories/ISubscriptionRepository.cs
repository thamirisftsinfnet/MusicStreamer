using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Interfaces.Repositories
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Task<Subscription> GetByUserIdAsync(int userId);
        Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
        Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync();
        Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysFromNow);
    }
}

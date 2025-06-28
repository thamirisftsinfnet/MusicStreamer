using MusicStreamer.Application.DTOs;
using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Interfaces.AppServices
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createSubscriptionDto);
        Task<SubscriptionDto> GetUserSubscriptionAsync(int userId);
        Task<SubscriptionDto> UpdateSubscriptionPlanAsync(int userId, SubscriptionPlanType newPlanType);
        Task CancelSubscriptionAsync(int userId);
        Task<bool> IsUserSubscriptionActiveAsync(int userId);
    }
}

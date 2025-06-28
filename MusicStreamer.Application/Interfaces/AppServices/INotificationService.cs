using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendTransactionNotificationAsync(Transaction transaction);
        Task SendWelcomeEmailAsync(User user);
    }
}

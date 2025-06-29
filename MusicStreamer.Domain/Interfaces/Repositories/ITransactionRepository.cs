using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Interfaces.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 20);
        Task<Transaction> GetLastAuthorizedTransactionAsync(int userId);
        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalSpentTodayAsync(int userId);
        Task<decimal> GetTotalSpentThisMonthAsync(int userId);
        Task<bool> HasRecentTransactionAsync(int userId, string merchantName, TimeSpan timeWindow);
    }
}

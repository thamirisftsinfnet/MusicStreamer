using Microsoft.EntityFrameworkCore;
using MusicStreamer.Data.Context;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Data.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(MusicStreamerContext context) : base(context)
        {
                
        }
        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 20)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Transaction> GetLastAuthorizedTransactionAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.Status == TransactionStatus.Authorized)
                .OrderByDescending(t => t.AuthorizedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId &&
                           t.CreatedAt >= startDate &&
                           t.CreatedAt <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSpentTodayAsync(int userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _context.Transactions
                .Where(t => t.UserId == userId &&
                           t.Status == TransactionStatus.Authorized &&
                           t.CreatedAt >= today &&
                           t.CreatedAt < tomorrow)
                .SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalSpentThisMonthAsync(int userId)
        {
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            return await _context.Transactions
                .Where(t => t.UserId == userId &&
                           t.Status == TransactionStatus.Authorized &&
                           t.CreatedAt >= firstDayOfMonth &&
                           t.CreatedAt < firstDayOfNextMonth)
                .SumAsync(t => t.Amount);
        }

        public async Task<bool> HasRecentTransactionAsync(int userId, string merchantName, TimeSpan timeWindow)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(timeWindow);

            return await _context.Transactions
                .AnyAsync(t => t.UserId == userId &&
                              t.MerchantName == merchantName &&
                              t.Status == TransactionStatus.Authorized &&
                              t.CreatedAt >= cutoffTime);
        }
    }
}

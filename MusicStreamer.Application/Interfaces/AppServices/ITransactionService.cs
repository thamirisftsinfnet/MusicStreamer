using MusicStreamer.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Interfaces.AppServices
{
    public interface ITransactionService
    {
        Task<TransactionAuthorizationResult> AuthorizeTransactionAsync(int id);
        Task<TransactionDto> GetTransactionByIdAsync(int transactionId);
        Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 20);
        Task<decimal> GetUserSpendingTodayAsync(int userId);
        Task<decimal> GetUserSpendingThisMonthAsync(int userId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<TransactionAuthorizationResult> AddTransactionAsync(AuthorizeTransactionDto authorizeTransactionDto);
    }
}

using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs;
using MusicStreamer.Application.Interfaces.AppServices;

namespace MusicStreamer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("{id}/authorize")]
        public async Task<IActionResult> Authorize(int id)
        {
            try
            {
                var result = await _transactionService.AuthorizeTransactionAsync(id);
                if (!result.IsAuthorized)
                    return BadRequest(new { result.DenialReason });

                return Ok(result.Transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
          
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {
                var transactions = await _transactionService.GetUserTransactionsAsync(userId);

                var result = transactions.Select(tx => new
                {
                    tx.Id,
                    tx.Amount,
                    tx.Description,
                    tx.MerchantName,
                    Status = tx.Status.ToString(),
                    tx.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }

}

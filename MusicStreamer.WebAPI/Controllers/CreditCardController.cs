using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs;
using MusicStreamer.Application.Interfaces.AppServices;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/creditcards")]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardService _creditCardService;

        public CreditCardController(ICreditCardService creditCardService)
        {
            _creditCardService = creditCardService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {
                var cards = await _creditCardService.GetByUserIdAsync(userId);
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] CreditCardDto dto)
        {
            try
            {
                var created = await _creditCardService.AddCardAsync(dto);
                return CreatedAtAction(nameof(GetByUser), new { userId = created.UserId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var card = await _creditCardService.GetByIdAsync(id);
            await _creditCardService.DeleteCardAsync(card);
            return NoContent();
        }
    }
}

using MusicStreamer.Application.DTOs;
using MusicStreamer.Application.Interfaces.AppServices;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Domain.Interfaces.UnitOfWork;
using MusicStreamer.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.AppServices
{
    public class CreditCardService : ICreditCardService
    {
        private readonly ICreditCardRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreditCardService(ICreditCardRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CreditCardDto>> GetByUserIdAsync(int userId)
        {
            var cards = await _repository.GetByUserIdAsync(userId);
            return cards.Select(c => new CreditCardDto
            {
                Id = c.Id,
                UserId = c.UserId,
                CardHolderName = c.CardHolderName,
                NumberMasked = c.NumberMasked,
                Expiration = c.Expiration,
                Brand = c.Brand,
                Token = c.Token
            });
        }
        public async Task<CreditCardDto> GetByIdAsync(int id)
        {
            var card = await _repository.GetByIdAsync(id);
            if (card == null)
                return null;

            return new CreditCardDto
            {
                Id = card.Id,
                UserId = card.UserId,
                CardHolderName = card.CardHolderName,
                NumberMasked = card.NumberMasked,
                Expiration = card.Expiration,
                Brand = card.Brand,
                Token = card.Token
            };
        }


        public async Task<CreditCardDto> AddCardAsync(CreditCardDto dto)
        {
            var card = new CreditCard
            {
                UserId = dto.UserId,
                CardHolderName = dto.CardHolderName,
                NumberMasked = dto.NumberMasked,
                Expiration = dto.Expiration,
                Brand = dto.Brand,
                Token = dto.Token
            };
            await _repository.AddAsync(card);
            await _unitOfWork.CommitAsync();

            dto.Id = card.Id;
            return dto;
        }

        public async Task DeleteCardAsync(CreditCardDto dto)
        {
            var card = new CreditCard
            {
                UserId = dto.UserId,
                CardHolderName = dto.CardHolderName,
                NumberMasked = dto.NumberMasked,
                Expiration = dto.Expiration,
                Brand = dto.Brand,
                Token = dto.Token
            };
            _repository.Remove(card);
            await _unitOfWork.CommitAsync();
        }
    }
}

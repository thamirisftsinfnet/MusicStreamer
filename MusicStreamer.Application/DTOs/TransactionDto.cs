using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MerchantName { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public TransactionStatus Status { get; set; }
        public string AuthorizationCode { get; set; }
        public string Description { get; set; }
        public DateTime? AuthorizedAt { get; set; }
    }

    public class CreateTransactionDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string MerchantName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class AuthorizeTransactionDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string MerchantName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class TransactionAuthorizationResult
    {
        public bool IsAuthorized { get; set; }
        public string AuthorizationCode { get; set; }
        public string DenialReason { get; set; }
        public TransactionDto Transaction { get; set; }
    }
}

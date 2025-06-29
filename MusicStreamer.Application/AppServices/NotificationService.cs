using Microsoft.Extensions.Configuration;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _config;

        public NotificationService(IConfiguration config)
        {
            _config = config;
        }

        public Task SendWelcomeEmailAsync(User user)
        {
            var subject = "Bem-vindo ao MusicStreamer!";
            var body = $"Olá {user.FirstName},\n\nSua conta foi criada com sucesso. Aproveite para explorar suas músicas favoritas!\n\nEquipe MusicStreamer 🎧";

            return SendEmailAsync(user.Email, subject, body);
        }

        public Task SendTransactionNotificationAsync(Transaction transaction)
        {
            var subject = $"Transação autorizada - {transaction.MerchantName}";
            var body = $"Olá,\n\nUma transação foi realizada no valor de R$ {transaction.Amount} com o comerciante {transaction.MerchantName}.\n\nData: {transaction.CreatedAt}\n\nCódigo: {transaction.AuthorizationCode}";

            return SendEmailAsync(transaction.User.Email, subject, body);
        }
        public Task SendTransactionPendenteNotificationAsync(Transaction transaction)
        {
            var subject = $"Transação Pendente - {transaction.MerchantName}";
            var body = $"Olá,\n\nUma transação foi realizada no valor de R$ {transaction.Amount} com o comerciante {transaction.MerchantName}.\n\nData: {transaction.CreatedAt}\n\nCódigo: {transaction.AuthorizationCode}";

            return SendEmailAsync(transaction.User.Email, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"]);
            var smtpUser = _config["Smtp:Username"];
            var smtpPass = _config["Smtp:Password"];
            var sender = _config["Smtp:Sender"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage(sender, to, subject, body);
            await client.SendMailAsync(mail);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using FABPortfolioApp.API.Dtos;

namespace FABPortfolioApp.API.Data
{
    public interface IUtilService
    {
        Task SendEmail(UserForEmailDto userEmail);
    }

    public class UtilService : IUtilService
    {
        private readonly IConfiguration _configuration;

        public UtilService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(UserForEmailDto userEmail)
        {
            using (var client = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = _configuration["Email:Email"],
                    Password = _configuration["Email:Password"]
                };

                client.Credentials = credential;
                client.Host = _configuration["Email:Host"];
                client.Port = int.Parse(_configuration["Email:Port"]);
                client.EnableSsl = true;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(userEmail.UserEmail));
                    emailMessage.From = new MailAddress(_configuration["Email:Email"]);
                    emailMessage.Subject = userEmail.Subject;
                    emailMessage.Body = userEmail.Message;
           
                    client.Send(emailMessage);
                }
            }
            await Task.CompletedTask;
        }


        

    }
}
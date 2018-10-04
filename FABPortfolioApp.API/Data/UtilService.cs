using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using FABPortfolioApp.API.Dtos;
using FABPortfolioApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MaxMind.GeoIP2;

namespace FABPortfolioApp.API.Data
{
    public interface IUtilService
    {
        Task SendEmail(UserForEmailDto userEmail);

        // Log Repo
        Task<IEnumerable<Log>> GetLogs(int pageNumber, int pageSize);
        int GetTotalLogCount();
        Task<Log> GetLogById(int id);
        bool IPDateExist(string ipAddress);
        void Add<T>(T entity) where T : class;
        Task<bool> SaveAll();
        void Delete<T>(T entity) where T : class;
    }

    public class UtilService : IUtilService
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public UtilService(
            IConfiguration configuration,
            DataContext context)
        {
            _context = context;
            _configuration = configuration;
        }



        #region Visitor Log Repo Implementation

        public async Task<IEnumerable<Log>> GetLogs(int pageNumber, int pageSize)
        {
            var logs = await _context.Logs
                        .OrderByDescending( o => o.DateLogged )
                        .Skip( (pageNumber - 1) * pageSize )
                        .Take( pageSize )
                        .ToListAsync();

            return logs; 
        }

        public int GetTotalLogCount() {
            var logCount =  _context.Logs.Count();  
            return  logCount;
        }


        public async Task<Log> GetLogById(int id)
        {
            var logById = await _context.Logs.FirstOrDefaultAsync( l => l.Id == id );
            return logById; 
        }

        // Returns true if an log already exist based on the ipAddress and the current date 
        public bool IPDateExist(string ipAddress)
        {
            return  (_context.Logs.Where(g => g.IPAddress == ipAddress.ToString() && g.DateLogged.Date == DateTime.Now.Date).Count() > 0) ? true : false;
        }




        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        #endregion

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
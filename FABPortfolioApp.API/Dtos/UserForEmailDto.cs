using System;
using System.ComponentModel.DataAnnotations;

namespace FABPortfolioApp.API.Dtos
{
    public class UserForEmailDto
    {
        public string UserEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
using System;
using System.Threading.Tasks;
using FABPortfolioApp.API.Data;
using FABPortfolioApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FABPortfolioApp.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUtilService _utilService;
        public UserController(IUtilService utilService)
        {
            _utilService = utilService;
        }


        // POST         api/user/email
        // Description  send email to me
        [HttpPost]
        [Route("email")]
        // string email, string subject, string message
        public async Task<IActionResult> SendEmailAsync(UserForEmailDto userEmail)
        {
            // throw new Exception (userEmail.UserEmail + " " + userEmail.Subject + " " + userEmail.Message); 
            await _utilService.SendEmail(userEmail);
            return Ok();
        }

    }
}
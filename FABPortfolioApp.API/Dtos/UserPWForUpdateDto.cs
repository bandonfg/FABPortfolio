using System;
using System.ComponentModel.DataAnnotations;

namespace FABPortfolioApp.API.Dtos
{
    public class UserPWForUpdateDto
    {   [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "You must specify a password between 6 and 8 characters")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "You must specify a password between 6 and 8 characters")]
        public string NewPassword { get; set; }
    }
}
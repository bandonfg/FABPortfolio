using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using FABPortfolioApp.API.Data;
using FABPortfolioApp.API.Models;
using Microsoft.AspNetCore.Identity;

namespace FABPortfolio.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly DataContext _context;
        public Seed(
            UserManager<User> userManager, 
            RoleManager<Role> roleManager,
            DataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void SeedPortfolios()
        {
            if (!_context.Portfolios.Any())
            {
                var portfolioData = System.IO.File.ReadAllText("Data/PortfolioSeedData.json");
                var portfolios = JsonConvert.DeserializeObject<List<Portfolio>>(portfolioData);
                foreach (var portfolio in portfolios)
                {
                    _context.Portfolios.Add(portfolio);
                }

                _context.SaveChanges();
            }
        }


        public void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                
                // define user roles
                var roles = new List<Role>
                {
                    new Role{Name = "Member"},
                    new Role{Name = "Admin"},
                };

                // create user roles
                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }

                // create users
                foreach (var user in users)
                {
                    // _context.Users.Add(user);
                    _userManager.CreateAsync(user, "P@ssw0rd").Wait();
                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                }

                // define admin user
                /* 
                var adminUser = new User
                {
                    UserName = "Admin"
                };

                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"}).Wait();
                }
                */
            }
        }


        /*
        public void SeedPortfolioFiles()
        {
            if (!_context.PortfolioFiles.Any())
            {
                var portfolioFileData = System.IO.File.ReadAllText("Data/PortfolioFileSeedData.json");
                var portfolioFiles = JsonConvert.DeserializeObject<List<Portfolio>>(portfolioFileData);
                foreach (var portfolioFile in portfolioFiles)
                {
                    _context.Portfolios.Add(portfolioFile);
                }

                _context.SaveChanges();
            }
        }


        public void SeedUsers()
        {
            if (!_context.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();

                    _context.Users.Add(user);
                }

                _context.SaveChanges();
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        */

    }
}
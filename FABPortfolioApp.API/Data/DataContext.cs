using FABPortfolioApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FABPortfolioApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options){}

        // Initial Portfolio tables
        public DbSet<Portfolio> Portfolios {get; set;}
        public DbSet<PortfolioFile> PortfolioFiles {get; set;}
    }
}
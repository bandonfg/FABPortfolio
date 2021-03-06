using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FABPortfolioApp.API.Models;
using FABPortfolioApp.API.Data;
using System.Linq.Expressions;
using System;
using System.Linq;
using FABPortfolioApp.API.Dtos;
using FABPortfolioApp.API.Helpers;

namespace FABPortfolioApp.API.Data
{
    public interface IPortfolioRepository
    {
        // Task<IEnumerable<Portfolio>> GetPortfolios();
        Task<IEnumerable<Portfolio>> GetPortfolios(int pageNumber, int pageSize, string companyFilter);



        int GetPortfolioCount(string companyFilter);
        Task<Portfolio> GetPortfolioById(int id);
        Task<IEnumerable<String>> GetUniquePortfolioCompanyList();
        Task<PortfolioFile> GetPortfolioFileById(int id);
        Task<IEnumerable<PortfolioFile>> GetPortfolioFilesById(int id);
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        Task<bool> SaveAll();
        void Delete<T>(T entity) where T : class;
        void DeleteWhere<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class;
    }

    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly DataContext _context;

        public PortfolioRepository(DataContext context)
        {
            _context = context;
        }

        /*orig code without pagination
        public async Task<IEnumerable<Portfolio>> GetPortfolios()
        {
            var portfolios = await _context.Portfolios.Include(pf => pf.PortfolioFiles ).ToListAsync();
            return portfolios; 
        }
         */
        public async Task<IEnumerable<Portfolio>> GetPortfolios(int pageNumber, int pageSize, string companyFilter)
        {
            var portfolios = await _context.Portfolios
                    .Include(pf => pf.PortfolioFiles ).ToListAsync();

            if(string.IsNullOrEmpty(companyFilter))
            {
                portfolios = await _context.Portfolios
                    .Include(pf => pf.PortfolioFiles )
                    .OrderByDescending( o=>o.To )
                    .ThenByDescending( p => p.From )
                    .Skip( (pageNumber - 1) * pageSize )
                    .Take( pageSize )
                    .ToListAsync();
            }
            else 
            {
                portfolios = await _context.Portfolios
                    .Include(pf => pf.PortfolioFiles )
                    .Where( f => f.Company == companyFilter ) 
                    .OrderByDescending( o=>o.To )
                    .ThenByDescending( p => p.From )
                    .Skip( (pageNumber - 1) * pageSize )
                    .Take( pageSize )
                    .ToListAsync();
            }

            return portfolios; 
        }

        public int GetPortfolioCount(string companyFilter) {

            var folioCount = ( string.IsNullOrEmpty(companyFilter) ) ? _context.Portfolios.Count() : _context.Portfolios.Where( c => c.Company == companyFilter ).Count();;  
            return  folioCount;
        }


        public async Task<IEnumerable<PortfolioFile>> GetPortfolioFilesById(int id)
        {
            var portfolioFiles = await _context.PortfolioFiles.Where( p => p.PortfolioId == id ).ToListAsync();
            return portfolioFiles; 
        }


        public async Task<Portfolio> GetPortfolioById(int id)
        {
            var portfolio = await _context.Portfolios
                                          .Include(pf => pf.PortfolioFiles )
                                          .FirstOrDefaultAsync( p => p.Id == id );
            return portfolio; 
        }

        // get unit portfolio company list 
        public async Task<IEnumerable<String>> GetUniquePortfolioCompanyList()
        {
            var portfolio = await _context.Portfolios
                                          .Select( p => p.Company )  
                                          .Distinct()
                                          .ToListAsync();
            return portfolio; 
        }



        public async Task<PortfolioFile> GetPortfolioFileById(int id)
        {
            var portfolioFile = await _context.PortfolioFiles
                                        .FirstOrDefaultAsync( p => p.Id == id );
            return portfolioFile; 
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public void DeleteWhere<TEntity>(Expression<Func<TEntity, bool>> predicate = null) 
        where TEntity : class
        {
            var dbSet = _context.Set<TEntity>();
            if (predicate != null)
                dbSet.RemoveRange(dbSet.Where(predicate));
            else
                dbSet.RemoveRange(dbSet);

            _context.SaveChanges();
        } 

    }
}
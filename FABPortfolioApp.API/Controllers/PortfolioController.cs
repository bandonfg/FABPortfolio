using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FABPortfolioApp.API.Dtos;
using FABPortfolioApp.API.Data;
using FABPortfolioApp.API.Models;

namespace FABPortfolioApp.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;

        // constructor
        public PortfolioController(
               IPortfolioRepository repo, 
               IMapper mapper, 
               IHostingEnvironment hostingEnvironment)
        {
            _repo = repo;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }
        // end of constructor


        // GET api/values
        [HttpGet( Name="GetPortfolios" )]
        public async Task<IActionResult> GetPortfolios()
        {
            var portfoliosFromRepo = await _repo.GetPortfolios();
            // use custom DTos to limit fields to display
            // ex. var portfolios = _mapper.Map<IEnumerable<PortfolioForCreationDto>>(portfoliosFromRepo);
            var portfolios = _mapper.Map<IEnumerable<Portfolio>>(portfoliosFromRepo);
            return Ok(portfolios);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPortfolioById(int id)
        {
            var portfolio = await _repo.GetPortfolioById(id);
            return Ok(portfolio);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> CreatePortfolio(PortfolioForCreationDto portfolioForCreation)
        {
            var newPortfolio = _mapper.Map<Portfolio>(portfolioForCreation);
            _repo.Add<Portfolio>(newPortfolio);

            if ( await _repo.SaveAll() )
                return Ok();
                
             throw new Exception("Portfolio creation failed on save.");
        }

        // PUT api/portfolio/edit/5
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdatePortfolio(int id, PortfolioForUpdateDto portforlioForUpdateDto)
        {
           var portfolioToUpdate = _mapper.Map<Portfolio>(portforlioForUpdateDto);
            _repo.Update<Portfolio>(portfolioToUpdate);

            if ( await _repo.SaveAll() )
                return Ok();
                
            throw new Exception($"Updating portfolio {id} failed on save.");
        }

        // void         DeletePortfolioFile(string fileName)
        // Description  Permanently delete file from server
        // Params       fileName - server file name 
        public void DeletePortfolioFile(string fileName) {
            var webRoot = _hostingEnvironment.WebRootPath; 
            string fullPath = webRoot + "/images/" + fileName;
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        // DELETE       api/portfolio/srcTable/id
        // Description  Deletes portfolio files, portfolio and portfolio files records
        // Params       srcTable - 1 => Portfolio, 2 => PortfolioFile
        //              id       - can either refer to PortfolioId or PortfolioFileId    
        [HttpDelete("{srcTable}/{id}")]
        public async Task<IActionResult> Delete(int srcTable, int id)
        {
            if (srcTable == 1) {
                // delete portfolio and portfolio file(s)
                var portfolioToDelete = _mapper.Map<Portfolio>( await _repo.GetPortfolioById(id) );

                // first delete portfolio files from server
                foreach (PortfolioFile folioFile in portfolioToDelete.PortfolioFiles)
                {
                    DeletePortfolioFile(folioFile.FileName);
                }  

                // then delete records from portfolio and portfolioFiles tables
                _repo.Delete<Portfolio>(portfolioToDelete);
            }
            else {
                // delete selected portfolio file
                var portfolioFileToDelete = _mapper.Map<PortfolioFile>( await _repo.GetPortfolioFileById(id) );
                DeletePortfolioFile(portfolioFileToDelete.FileName);
                _repo.Delete<PortfolioFile>(portfolioFileToDelete);
            }

            if (await _repo.SaveAll())
                return Ok();

            throw new Exception("Error deleting portfolio id " + id);
        }
    }
}

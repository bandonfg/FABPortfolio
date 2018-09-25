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
using System.IO;
using System.Net.Http.Headers;

namespace FABPortfolioApp.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IPortfolioRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;

        int newEditPortfolioId;

        // constructor
        public PortfolioController(
               DataContext context, 
               IPortfolioRepository repo, 
               IMapper mapper, 
               IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }
        // end of constructor

        /*  
        ////////////////////////////////////////////////////////////////////////
        // Portfolio File Modules                                             //
        ////////////////////////////////////////////////////////////////////////
        */
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
                newEditPortfolioId = newPortfolio.Id;
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
                newEditPortfolioId = portfolioToUpdate.Id;
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
    



        /*  
        ////////////////////////////////////////////////////////////////////////
        // Portfolio File Modules                                             //
        ////////////////////////////////////////////////////////////////////////
        */


        // POST api/portfolio/file
        [HttpPost("file")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {

                var file = Request.Form.Files[0];
                string folderName = "images";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string fileName = "";
                string fullPath = "";

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                // create portfolio file data
                var portfolioFileForCreation = new PortfolioFileForCreationDto(); 
                portfolioFileForCreation.FileName = fileName;
                portfolioFileForCreation.PortfolioId = newEditPortfolioId;      

                var portfolio = await _repo.GetPortfolioById(newEditPortfolioId); 
                var folioFile = _mapper.Map<PortfolioFile>(portfolioFileForCreation);
                folioFile.Portfolio = portfolio;    

                _repo.Add<PortfolioFile>(folioFile);
                if ( await _repo.SaveAll() )
                    return Ok("Upload Successful.");
                    
                throw new Exception("Portfolio creation failed on save.");
            
            }
            catch (System.Exception ex)
            {
                // return Json("Upload Failed: " + ex.Message);
                return BadRequest("Upload Failed: " + ex.Message);
            }
        }


        // https://stackoverflow.com/questions/40214772/file-upload-in-angular
        // http://www.talkingdotnet.com/upload-file-angular-5-asp-net-core-2-1-web-api/
        // https://stackoverflow.com/questions/48339510/asp-net-core-2-webapi-post-related-data-insert
        public async Task<IActionResult> CreatePortfolioFile(PortfolioFileForCreationDto portfolioFileForCreation)
        {
            /* 
                var portfolioFileForCreation = new PortfolioFileForCreationDto(); 
                portfolioFileForCreation.FileName = fileName;
                portfolioFileForCreation.PortfolioId = newEditPortfolioId;      
            */

            var portfolio = await _repo.GetPortfolioById(portfolioFileForCreation.PortfolioId); 

            var folioFile = _mapper.Map<PortfolioFile>(portfolioFileForCreation);

            folioFile.Portfolio = portfolio;    

            _repo.Add<PortfolioFile>(folioFile);

            if ( await _repo.SaveAll() )
                return Ok();
                //return CreatedAtRoute("");
                
             throw new Exception("Portfolio creation failed on save.");
        }

    }
}

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
using FABPortfolioApp.API.Helpers;

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

        int portfolioIdForUpload;

        #region PortfolioController Constructor
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
        #endregion


        /*  
        ////////////////////////////////////////////////////////////////////////
        // Portfolio File Modules                                             //
        ////////////////////////////////////////////////////////////////////////
        */
        // sort, search, and paging features will be added here
        // GET api/portfolio
        
        /* Original Code without pagination 10012018 
        [HttpGet( Name="GetPortfolios" )]
        public async Task<IActionResult> GetPortfolios()
        {
            var portfoliosFromRepo = await _repo.GetPortfolios();
            // use custom DTos to limit fields to display
            // ex. var portfolios = _mapper.Map<IEnumerable<PortfolioForCreationDto>>(portfoliosFromRepo);
            var portfolios = _mapper.Map<IEnumerable<Portfolio>>(portfoliosFromRepo);
            return Ok(portfolios);
        }
        */
        // [HttpGet("{pageNumber}/{pageSize}", Name="GetPortfolios" )]
        [HttpGet(Name="GetPortfolios" )]
        public async Task<IActionResult> GetPortfolios(int pageNumber, int pageSize)
        {

            if (pageSize <= 0)
                pageSize = 2;
            if (pageNumber <= 0)
                pageNumber = 1;

            var portfoliosFromRepo = await _repo.GetPortfolios(pageNumber, pageSize);
            // use custom DTos to limit or add fields to display

            var portfoliosToReturn = new {
                TotalCount = _repo.GetPortfolioCount(),
                TotalPages = Math.Ceiling((double)portfoliosFromRepo.Count() / pageSize),
                Portfolios = portfoliosFromRepo
            };

            return Ok(portfoliosToReturn);
        }



        // GET          api/portfolio/{id}
        // Description  return specific portfolio and file(s)
        [HttpGet("{id}", Name="GetPortfolioById")]
        public async Task<IActionResult> GetPortfolioById(int id)
        {
            var portfolio = await _repo.GetPortfolioById(id);
            return Ok(portfolio);
        }


        // GET          api/portfolio/company
        // Description  return specific portfolio and file(s)
        [HttpGet("company")]
        public async Task<IActionResult> GetUniquePortfolioCompanies(int id)
        {
            var portfolio = await _repo.GetUniquePortfolioCompanyList();
            return Ok(portfolio);
        }


        // POST         api/portfolio
        // Description  Create or add portfolio entry
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePortfolio(PortfolioForCreationDto portfolioForCreation)
        {
            var newPortfolio = _mapper.Map<Portfolio>(portfolioForCreation);
            _repo.Add<Portfolio>(newPortfolio);

            if ( await _repo.SaveAll() )
                portfolioIdForUpload = newPortfolio.Id;
                return Ok(newPortfolio);
                
             throw new Exception("Portfolio creation failed on save.");
        }

        // PUT          api/portfolio/edit/5
        // Description  updates portfolio with an id of 5
        [Authorize]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdatePortfolio(int id, PortfolioForUpdateDto portforlioForUpdateDto)
        {
           var portfolioToUpdate = _mapper.Map<Portfolio>(portforlioForUpdateDto);
            _repo.Update<Portfolio>(portfolioToUpdate);

            if ( await _repo.SaveAll() )
                portfolioIdForUpload = portfolioToUpdate.Id;
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
        [Authorize]
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


        ////////////////////////////////////////////////////////////////////////
        // Portfolio File Modules                                             //
        ////////////////////////////////////////////////////////////////////////

        // GET          api/portfolio/file/{id}
        // Description  return specific portfolio and file(s)
        [HttpGet("file/{id}")]
        public async Task<IActionResult> GetPortfolioFilesById(int id)
        {
            var portfolioFiles = await _repo.GetPortfolioFilesById(id);
            return Ok(portfolioFiles);
        }


        // POST         api/portfolio/file/id
        // Description  if refers to portfolio id
        [Authorize]
        [HttpPost("file/{id}")]
        public async Task<IActionResult> UploadFile(int id)
        {
            try
            {   
                // for debug/test only
                // throw new Exception("UploadFile(id) -> " + id);

                // file upload routine //
                var file = Request.Form.Files[0];
                string folderName = "assets/images";
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
                // end of file upload routine //

         
                // add file info portfolio file table //
                PortfolioFile portfolioFileForCreation = new PortfolioFile(); 
                portfolioFileForCreation.FileName = fileName;

                if (id < 1)
                    portfolioFileForCreation.PortfolioId = portfolioIdForUpload;
                else 
                    portfolioFileForCreation.PortfolioId = id;

                var portfolio = await _repo.GetPortfolioById(id); 
                var folioFile = _mapper.Map<PortfolioFile>(portfolioFileForCreation);
                folioFile.Portfolio = portfolio;    

                _repo.Add<PortfolioFile>(folioFile);
                if ( await _repo.SaveAll() )
                    return CreatedAtRoute("GetPortfolioById", new {id = id}, portfolio);
                    // return Ok("Upload Successful.");
                    
                throw new Exception("Portfolio creation failed on save.");
                // end of add file info portfolio file table //
            }
            catch (System.Exception ex)
            {
                return BadRequest("Upload Failed: " + ex.Message);
                // throw new Exception("Upload Failed: " + ex.Message);
            }
        }


        // Note: Used for testing portfolio file info save routine
        //[HttpPost("file")]
        public async Task<IActionResult> SavePortfolioFileInfo(PortfolioFileForCreationDto folioFileDto) {
                ////////////////////////////////////////
                // add file info portfolio file table //
                ////////////////////////////////////////
                PortfolioFile portfolioFileForCreation = new PortfolioFile(); 
                portfolioFileForCreation.FileName = folioFileDto.FileName;
                portfolioFileForCreation.PortfolioId = folioFileDto.PortfolioId;      

                var portfolio = await _repo.GetPortfolioById(folioFileDto.PortfolioId); 
                var folioFile = _mapper.Map<PortfolioFile>(portfolioFileForCreation);
                folioFile.Portfolio = portfolio;    

                _repo.Add<PortfolioFile>(folioFile);
                if ( await _repo.SaveAll() )
                    return Ok();
                    // return CreatedAtRoute("GetPortfolioById", new {id=folioFileDto.PortfolioId}, portfolio);
                    // return Ok("Upload Successful.");
                    
                throw new Exception("Portfolio creation failed on save.");
                ///////////////////////////////////////////////
                // end of add file info portfolio file table //
                ///////////////////////////////////////////////
        }





        // https://stackoverflow.com/questions/40214772/file-upload-in-angular
        // http://www.talkingdotnet.com/upload-file-angular-5-asp-net-core-2-1-web-api/
        // https://stackoverflow.com/questions/48339510/asp-net-core-2-webapi-post-related-data-insert
        public async Task<IActionResult> CreatePortfolioFile(PortfolioFileForCreationDto portfolioFileForCreation)
        {
            /* 
                var portfolioFileForCreation = new PortfolioFileForCreationDto(); 
                portfolioFileForCreation.FileName = fileName;
                portfolioFileForCreation.PortfolioId = portfolioIdForUpload;      
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
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
using MaxMind.GeoIP2;

namespace FABPortfolioApp.API.Controllers
{
    // [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class GeoLogController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUtilService _repo;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;

        #region Log Construction
        public GeoLogController(
               DataContext context, 
               IUtilService utilService,
               IMapper mapper, 
               IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _repo = utilService;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion


        // GET          api/log
        // Description  get paged list of visitor logs arranged in descending order by DateLogged
        // Params       passed in spa client via header params   
        [HttpGet]
        public async Task<IActionResult> GetLogs(int pageNumber, int pageSize)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (pageNumber <= 0)
                pageNumber = 1;

            var logsFromRepo = await _repo.GetLogs( pageNumber, pageSize );

            var logsToReturn = new {
                TotalCount =  _repo.GetTotalLogCount(),
                TotalPages = Math.Ceiling((double)logsFromRepo.Count() / pageSize),
                Logs = logsFromRepo
            };

            return Ok(logsToReturn);
        }


        // POST         api/log
        // Description  Create or add log entry
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateLog()
        {
            // get visitor geolocation info 
            using (var reader = new DatabaseReader(_hostingEnvironment.ContentRootPath + "\\Data\\GeoLog\\GeoLite2-City.mmdb"))
            {
                // for  local dev test Determine the IP Address of the request
                // var ipAddress = "180.255.44.125";

                // get user/visitor ip address
                var ipAddress = HttpContext.Connection.RemoteIpAddress;

                // Get the city from the IP Address
                var log = reader.City(ipAddress);

                // ensure logging is done once a day only. Compares ip address and datelogged to current date
                // var IPDateExist = (_context.Logs.Where(g => g.IPAddress == ipAddress.ToString() && g.DateLogged.Date == DateTime.Now.Date).Count() > 0) ? true : false;
                var IPDateExist = _repo.IPDateExist(ipAddress.ToString()); 

                // when log does not exist, create or add log
                if (!IPDateExist)
                {
                    //save or log visitor info
                    Log geoLog = new Log();
                    geoLog.IPAddress = ipAddress.ToString();
                    geoLog.City = log.City.ToString();
                    geoLog.Country = log.Country.ToString();
                    geoLog.Timezone = log.Location.TimeZone;
                    geoLog.DateLogged = DateTime.Now;

                    var newLog = _mapper.Map<Log>(geoLog);
                    _repo.Add<Log>(newLog);

                    if ( await _repo.SaveAll() )
                        return Ok(newLog);
                    throw new Exception("Log creation failed on save.");
                }
                                        
                return Ok("ip already exist, no new log was created.");
            }
        }


        // DELETE       api/log/{id}
        // Description  delete log file
        // Params       id - log to be deleted
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // find log to delete
            var logToDelete = _mapper.Map<Log>( await _repo.GetLogById(id) );

            // then delete log
            _repo.Delete<Log>(logToDelete);
          
            if (await _repo.SaveAll())
                return Ok();

            throw new Exception("Error deleting log id " + id);
        }
        
    }
}
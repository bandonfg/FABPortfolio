using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using FABPortfolioApp.API.Data;
using FABPortfolioApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FABPortfolioApp.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _repo;
        private readonly IMapper _mapper;

        // constructor
        public PortfolioController(IPortfolioRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        // GET api/values
        [HttpGet]
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
        [HttpPost("/create")]
        public async Task<IActionResult> CreatePortfolio(PortfolioForCreationDto portfolioForCreation)
        {
            var newPortfolio = _mapper.Map<Portfolio>(portfolioForCreation);
            _repo.Add<Portfolio>(newPortfolio);

            if ( await _repo.SaveAll() )
                return Ok();
                
            return BadRequest();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

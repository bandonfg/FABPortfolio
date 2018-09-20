using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FABPortfolioApp.API.Data;
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

        // constructor
        public PortfolioController(IPortfolioRepository repo)
        {
            _repo = repo;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetPortfolios()
        {
            var portfolios = await _repo.GetPortfolios();
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
        public void Post([FromBody]string value)
        {
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

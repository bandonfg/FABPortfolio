using System;
using System.Collections.Generic;
using FABPortfolioApp.API.Models;

namespace FABPortfolioApp.API.Dtos
{
    public class PortfolioForListDto
    {
        public int Id { get; set; }
        public string Project { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Url { get; set; }
        public double TotalCount { get; set; }
        public double TotalPages { get; set; }
        public ICollection<PortfolioFile> PortfolioFiles { get; set; }
    }
}


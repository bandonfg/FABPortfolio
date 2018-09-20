using System;
using System.Collections.Generic;

namespace FABPortfolioApp.API.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Project { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Url { get; set; }
        public ICollection<PortfolioFile> PortfolioFiles { get; set; }
    }
}


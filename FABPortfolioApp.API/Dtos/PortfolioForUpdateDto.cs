using System;

namespace FABPortfolioApp.API.Dtos
{
    public class PortfolioForUpdateDto
    {
        public int Id { get; set; }
        public string Project { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Url { get; set; }
    }
}
using System;

namespace FABPortfolioApp.API.Dtos
{
    public class PortfolioFileForCreationDto
    {
        public int PortfolioId { get; set; }
        public string FileName { get; set; }
        public int OrderId { get; set; }
    }

}
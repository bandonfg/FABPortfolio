using System;

namespace FABPortfolioApp.API.Dtos
{
    public class PortfolioFileForListDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public string FileName { get; set; }
        public int OrderId { get; set; }
    }

}
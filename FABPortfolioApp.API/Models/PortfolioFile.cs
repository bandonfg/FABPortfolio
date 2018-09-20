namespace FABPortfolioApp.API.Models
{
    public class PortfolioFile
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public string FileName { get; set; }
        public int OrderId { get; set; }
        public string Description { get; set; }
        public Portfolio Portfolio { get; set; }

    }
}
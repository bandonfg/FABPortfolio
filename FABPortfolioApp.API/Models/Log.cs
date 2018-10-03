using System;

namespace FABPortfolioApp.API.Models
{
    public class Log
    {
        #region Log Properties
        public int Id { get; set; }

        public string IPAddress { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Timezone { get; set; }

        public DateTime DateLogged { get; set; }

        #endregion
    }
}
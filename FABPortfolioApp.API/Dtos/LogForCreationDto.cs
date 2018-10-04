using System;

namespace FABPortfolioApp.API.Dtos
{
    public class LogForCreationDto
    {
        public string IPAddress { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Timezone { get; set; }

        public DateTime DateLogged { get; set; }
    }

}
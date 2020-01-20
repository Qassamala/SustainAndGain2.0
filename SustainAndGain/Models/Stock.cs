using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class Stock
    {
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string CEO { get; set; }
        public string Sector { get; set; }
        public bool IsSustainable { get; set; }


    }
}

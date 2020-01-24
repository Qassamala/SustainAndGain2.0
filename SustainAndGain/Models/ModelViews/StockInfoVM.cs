using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class StockInfoVM
    {
        public string CompanyName { get; set; }
        public bool? IsSustainable { get; set; }
        public string Symbol { get; set; }
        public decimal LastPrice { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal ReturnThisYear { get; set; }
        public string Description { get; set; }
        public string Sector { get; set; }
    }
}

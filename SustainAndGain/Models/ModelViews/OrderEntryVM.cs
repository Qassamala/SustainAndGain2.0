using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class OrderEntryVM
    {
        public string CompanyName { get; set; }
        public string Symbol { get; set; }
        public decimal OrderValue { get; set; }
        public decimal LastPrice { get; set; }
    }
}

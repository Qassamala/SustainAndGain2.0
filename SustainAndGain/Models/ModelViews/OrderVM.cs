using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class OrderVM
    {
        public string Symbol { get; set; }
        public decimal OrderValue { get; set; }
        public DateTime TimeOfInsertion { get; set; }
        public string BuyOrSell { get; set; }
    }
}

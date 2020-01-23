using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class PortfolioVM
    {
        public decimal CurrentValue { get; set; }
        public decimal AvailableCapital { get; set; }
        public decimal InvestedCapital { get; set; }
        //public List<Order> ListOfOrders { get; set; }
    }
}

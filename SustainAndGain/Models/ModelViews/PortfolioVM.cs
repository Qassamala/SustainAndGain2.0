using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class PortfolioVM
    {
        [Display(Name = "Current value")]
        public decimal CurrentValue { get; set; }

        [Display(Name = "Available capital")]
        public decimal AvailableCapital { get; set; }

        [Display(Name = "Invested capital")]
        public decimal InvestedCapital { get; set; }

        //[Display(Name = "List of pending orders")]
        //public List<Order> ListOfOrders { get; set; }
    }
}

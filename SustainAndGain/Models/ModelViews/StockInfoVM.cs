using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class StockInfoVM
    {
        [Display(Name ="Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Sustainable")]
        public bool? IsSustainable { get; set; }
    
        public string Symbol { get; set; }
        [Display(Name = "Last Price")]
        public decimal LastPrice { get; set; }
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }
        [Display(Name = "Return This Year")]
        public decimal ReturnThisYear { get; set; }

        public string Description { get; set; }
        public string Sector { get; set; }
        public int CompetitionId { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SustainAndGain.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{

    public class SellOrderVM
    {

        public int StockId { get; set; }
        public int CompetitionId { get; set; }

        public int OrderId { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Symbol")]
        [Required(ErrorMessage = "Must enter a valid symbol")]
        public string Symbol { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Must enter a quantity")]
        public int Quantity { get; set; }

        public DateTime TimeOfInsertion { get; set; }
        [Display(Name = "Buy Or Sell")]
        public string BuyOrSell { get; set; }

        [Display(Name = "Last Price")]
        public decimal LastPrice { get; set; }

    }
}

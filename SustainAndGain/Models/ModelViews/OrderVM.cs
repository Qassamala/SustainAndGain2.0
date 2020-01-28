using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class OrderVM
    {
        public int StockId { get; set; }
        public int CompetitionId { get; set; }

        public int OrderId { get; set; }

        [Display(Name ="Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Symbol")]
        [Required(ErrorMessage = "Must enter a valid symbol")]
        public string Symbol { get; set; }

        [Display(Name = "Order Value")]
        [Required(ErrorMessage = "Must enter an order value")]
        [Range(1, int.MaxValue, ErrorMessage = "Must be bigger than 0")]
        public decimal OrderValue { get; set; }

        public DateTime TimeOfInsertion { get; set; }
        [Display(Name = "Buy Or Sell")]
        public string BuyOrSell { get; set; }

        [Display(Name = "Available to Invest")]
        public decimal AvailableToInvest { get; set; }

        [Display(Name = "Last Price")]
        public decimal LastPrice { get; set; }

    }
}

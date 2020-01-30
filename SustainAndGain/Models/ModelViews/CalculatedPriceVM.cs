using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
	public class CalculatedPriceVM
	{
        public string Symbol { get; set; }

        [Display(Name = "Company name")]
        public string CompanyName { get; set; }

        [Display(Name = "Sustainable")]
        public bool? IsSustainable { get; set; }
        public string BuyOrSell { get; set; }

        [Display(Name = "Purchase price")]
        public decimal PurchasePrice { get; set; }

        [Display(Name = "Current price")]
        public decimal CurrentPrice { get; set; }

        [Display(Name = "Total quantity")]
        public int TotalQuantity { get; set; }

        [Display(Name = "Market value")]
        public decimal MarketValue => TotalQuantity * CurrentPrice;
        public decimal Return => (CurrentPrice - PurchasePrice)/PurchasePrice*100;

        [Display(Name = "Average price")]
        public decimal AveragePrice  { get; set; }

        public int Id { get; set; }
        public string UserId { get; set; }
        public int CompetitionId { get; set; }
        public int StockId { get; set; }
        public int Quantity { get; set; }
        public decimal TransactionPrice { get; set; }
        public DateTime DateTimeOfTransaction { get; set; }
        public int CurrentHoldingsAfterTransaction { get; set; }

        public string Description { get; set; }


    }
}

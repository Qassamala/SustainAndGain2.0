using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
	public class CalculatedPriceVM
	{
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public bool? IsSustainable { get; set; }
        public string BuyOrSell { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public int TotalQuantity { get; set; }
        public decimal MarketValue => TotalQuantity * CurrentPrice;
        public decimal Return { get; set; }

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

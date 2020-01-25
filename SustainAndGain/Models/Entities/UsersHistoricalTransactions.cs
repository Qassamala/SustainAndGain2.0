using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class UsersHistoricalTransactions
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CompetitionId { get; set; }
        public int StockId { get; set; }
        public int Quantity { get; set; }
        public decimal TransactionPrice { get; set; }
        public DateTime DateTimeOfTransaction { get; set; }
        public int CurrentHoldingsAfterTransaction { get; set; }
        public string BuyOrSell { get; set; }

        public virtual Competition Competition { get; set; }
        public virtual StaticStockData Stock { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}

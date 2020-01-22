using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class HistDataStocks
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal? CurrentPrice { get; set; }
        public string Symbol { get; set; }

        public virtual StaticStockData Stock { get; set; }
    }
}

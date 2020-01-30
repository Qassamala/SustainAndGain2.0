using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class StaticStockData
    {
        public StaticStockData()
        {
            HistDataStocks = new HashSet<HistDataStocks>();
            Order = new HashSet<Order>();
            UsersHistoricalTransactions = new HashSet<UsersHistoricalTransactions>();
        }

        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public string Sector { get; set; }
        public string CompanyName { get; set; }
        public bool? IsSustainable { get; set; }

        public virtual ICollection<HistDataStocks> HistDataStocks { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<UsersHistoricalTransactions> UsersHistoricalTransactions { get; set; }
    }
}

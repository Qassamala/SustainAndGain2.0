using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class StaticStockData
    {
        public StaticStockData()
        {
            HistDataStocks = new HashSet<HistDataStocks>();
            StocksInCompetition = new HashSet<StocksInCompetition>();
            UsersHistoricalTransactions = new HashSet<UsersHistoricalTransactions>();
        }

        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public string Sector { get; set; }
        public string CompanyName { get; set; }

        public virtual ICollection<HistDataStocks> HistDataStocks { get; set; }
        public virtual ICollection<StocksInCompetition> StocksInCompetition { get; set; }
        public virtual ICollection<UsersHistoricalTransactions> UsersHistoricalTransactions { get; set; }
    }
}

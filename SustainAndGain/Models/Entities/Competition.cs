using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class Competition
    {
        public Competition()
        {
            StocksInCompetition = new HashSet<StocksInCompetition>();
            UsersHistoricalTransactions = new HashSet<UsersHistoricalTransactions>();
        }

        public int Id { get; set; }
        public bool IsOngoing { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual ICollection<StocksInCompetition> StocksInCompetition { get; set; }
        public virtual ICollection<UsersHistoricalTransactions> UsersHistoricalTransactions { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class Competition
    {
        public Competition()
        {
            Order = new HashSet<Order>();
            StocksInCompetition = new HashSet<StocksInCompetition>();
            UsersHistoricalTransactions = new HashSet<UsersHistoricalTransactions>();
            UsersInCompetition = new HashSet<UsersInCompetition>();
        }

        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<StocksInCompetition> StocksInCompetition { get; set; }
        public virtual ICollection<UsersHistoricalTransactions> UsersHistoricalTransactions { get; set; }
        public virtual ICollection<UsersInCompetition> UsersInCompetition { get; set; }
    }
}

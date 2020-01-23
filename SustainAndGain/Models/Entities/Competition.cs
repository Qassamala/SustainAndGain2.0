using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class Competition
    {
        public Competition()
        {
            UsersHistoricalTransactions = new HashSet<UsersHistoricalTransactions>();
            UsersInCompetition = new HashSet<UsersInCompetition>();
        }

        public int Id { get; set; }
        public bool IsOngoing { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UsersHistoricalTransactions> UsersHistoricalTransactions { get; set; }
        public virtual ICollection<UsersInCompetition> UsersInCompetition { get; set; }
    }
}

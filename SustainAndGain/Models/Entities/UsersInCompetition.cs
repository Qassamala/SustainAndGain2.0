using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class UsersInCompetition
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CompId { get; set; }
        public decimal CurrentValue { get; set; }

        public virtual Competition Comp { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}

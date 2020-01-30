using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class BonusDeposit
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CompetitionId { get; set; }
        public decimal? Bonus { get; set; }

        public virtual Competition Competition { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}

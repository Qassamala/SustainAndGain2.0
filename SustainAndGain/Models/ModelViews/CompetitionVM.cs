using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class CompetitionVM
    {
        public CompetitionVM()
        {
            if (StartTime < DateTime.Now)
            {
                IsOngoing = true;
            }
        }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsOngoing { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}

using SustainAndGain.Models.Entities;
using SustainAndGain.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class CompetitionsService
    {
        private readonly SustainGainContext context;

        public CompetitionsService(SustainGainContext context)
        {
            this.context = context;
        }

        public void AddCompetition()
        {
            int month = 1;
            
            for (int i = 0; i < 4; i++)
                
            {
                Competition competition = new Competition
                {
                    StartTime = new DateTime(2020, month, 23),
                    EndTime = new DateTime(2020, month + 1, 23),
                    Name = "Hello",
                    IsOngoing = true
                  
                    

                };
                month++;
                context.Competition.Add(competition);
                context.SaveChanges();
            }
        }

        public CompetitionVM[] DisplayCompetitions()
        {

         

            List<CompetitionVM> competitions = new List<CompetitionVM>();

            foreach (var item in context.Competition)
            {
                CompetitionVM competition = new CompetitionVM
                {
                    EndTime = item.EndTime,
                    StartTime = item.StartTime,
                    Name = item.Name,
                    IsOngoing = item.IsOngoing

                };
                competitions.Add(competition);

            }
            return competitions.ToArray();


          

        }

    }
}

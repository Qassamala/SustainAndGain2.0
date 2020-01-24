using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SustainAndGain.Models.Entities;
using SustainAndGain.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class PortfoliosService
    {
		private readonly SustainGainContext context;
		private readonly UserManager<MyIdentityUser> user;
		private readonly IHttpContextAccessor accessor;

		public PortfoliosService(SustainGainContext context, UserManager<MyIdentityUser> user, IHttpContextAccessor accessor)
		{
			this.context = context;
			this.user = user;
			this.accessor = accessor;
		}

		public PortfolioVM DisplayPortfolioData(int compId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			var lastupdatedCurrentValue = context.UsersInCompetition.Where(o => ((o.CompId == compId) && (o.UserId == userId))).Max(o => o.LastUpdatedCurrentValue);

			var currentValue = context.UsersInCompetition.Where(o => o.LastUpdatedCurrentValue == lastupdatedCurrentValue).Select(v => v.CurrentValue).SingleOrDefault();


			var lastupdatedAvailableForInvestment = context.UsersInCompetition.Where(o => ((o.CompId == compId) && (o.UserId == userId))).Max(o => o.LastUpdatedAvailableForInvestment);

			var availableForInvestment = context.UsersInCompetition.Where(o => o.LastUpdatedCurrentValue == lastupdatedAvailableForInvestment).Select(v => v.AvailableForInvestment).SingleOrDefault();
			
			PortfolioVM portfolioData = new PortfolioVM { CurrentValue = (decimal)currentValue, AvailableCapital = (decimal)availableForInvestment, InvestedCapital = (decimal)(currentValue - availableForInvestment) };

			return portfolioData;
		}

		//public decimal GetCurrentValue()
		//{
		//	AvailableCapital
		//}
	}
}

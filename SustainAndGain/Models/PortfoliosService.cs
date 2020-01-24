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

		//public PortfolioVM DisplayPortfolioData()
		//{
			//string userId = user.GetUserId(accessor.HttpContext.User);

			//decimal currentValue = context.UsersInCompetition.(v => v.LastUpdatedCurrentValue)

			//PortfolioVM viewModel = new PortfolioVM {CurrentValue = , AvailableCapital, InvestedCapital };

			//return viewModel;
		//}
		
		//public decimal GetCurrentValue()
		//{
		//	AvailableCapital
		//}
	}
}

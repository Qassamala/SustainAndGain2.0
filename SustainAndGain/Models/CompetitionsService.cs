﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<MyIdentityUser> user;
		private readonly IHttpContextAccessor accessor;

		public CompetitionsService(SustainGainContext context, UserManager<MyIdentityUser> user, IHttpContextAccessor accessor)
		{
			this.context = context;
			this.user = user;
			this.accessor = accessor;
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
						Name = $"Nr {month}",
					};

				month++;
				context.Competition.Add(competition);
				context.SaveChanges();
			}
		}

		public CompetitionVM[] DisplayCompetitions()
		{			
			string userId = user.GetUserId(accessor.HttpContext.User);
			DateTime now = DateTime.Now;
			return context.Competition
				.Select(item => new CompetitionVM
				{
					EndTime = item.EndTime,
					DaysLeft =item.StartTime.Subtract(now).TotalDays.ToString("0"),
					Name = item.Name,
					Id = item.Id,
					UserId = userId,
					HasJoined = item.UsersInCompetition.Any(o => o.UserId == userId)
				}).ToArray();
		}
	

	}
}

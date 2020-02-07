using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SustainAndGain.Controllers
{
    [Authorize]
    public class ErrorsController : Controller
    {

        [Route("error/servererror")]
        public IActionResult ServerError()
        {
            var error = HttpContext.Features.Get
                <IExceptionHandlerPathFeature>()?.Error;

            if (error != null)
            {
                //Write error to db table errors
            }
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class UserLoginVM
    { 
        [Required]
        [Display(Name ="Username")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace SustainAndGain.Models.ModelViews
{
    class MaxValueAttribute : ValidationAttribute
    {
        int maxValue;

        public MaxValueAttribute(int maxValue)
        {
            this.maxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            try
            {
                if (value != null)
                {
                    return (int)value == maxValue;

                }
            }
            catch (Exception) { }
            {

                return false;
            }
        }
    }


}
using DatabaseCommunicator.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TimeControlSettingsListMovesOrderedAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates if the time control settings is from the lowest move to the highest
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is List<TimeControlSettingsPiece> values)
            {
                for(int i = 0; i < values.Count - 1; i++)
                {
                    if (values[i].UntilMove >= values[i + 1].UntilMove)
                    {
                        return new ValidationResult("");
                    }
                }
                return ValidationResult.Success;
            }

            throw new ValidationException("This attribute can be added only to List<TimeControlSettingsPiece>");         
        }
    }
}

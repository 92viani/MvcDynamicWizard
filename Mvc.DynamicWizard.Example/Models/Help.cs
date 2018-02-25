using Mvc.DynamicWizard.ViewModels;
using System;

namespace Mvc.DynamicWizard.Sample.Models
{
    [Serializable]
    public class Help : StepViewModel
    {
        public Help() { }
        public Help(string stepTitle,string stepIconCss)
        {
            Title = stepTitle;
            Icon = stepIconCss;
            HelpDescription =   
                @"Commonly Used Types:
System.ComponentModel.DataAnnotations.ValidationResult
System.ComponentModel.DataAnnotations.IValidatableObject
System.ComponentModel.DataAnnotations.ValidationAttribute
System.ComponentModel.DataAnnotations.RequiredAttribute
System.ComponentModel.DataAnnotations.StringLengthAttribute
System.ComponentModel.DataAnnotations.DisplayAttribute
System.ComponentModel.DataAnnotations.RegularExpressionAttribute
System.ComponentModel.DataAnnotations.DataTypeAttribute
System.ComponentModel.DataAnnotations.RangeAttribute
System.ComponentModel.DataAnnotations.KeyAttribute";
        }

        public string HelpDescription { get; set; }
    }
}
using Mvc.DynamicWizard.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mvc.DynamicWizard.Sample.Models
{
    [Serializable]
    public class PersonInfo : StepViewModel
    {
        public PersonInfo() { }
        public PersonInfo(string stepTitle,string stepIconCss)
        {
            Title = stepTitle;
            Icon = stepIconCss;
        }

        [Required]
        public string NationalNo { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Family { get; set; }

        public string PhoneNmber { get; set; }
    }
}
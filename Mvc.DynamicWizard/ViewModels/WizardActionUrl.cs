using System;

namespace Mvc.DynamicWizard.ViewModels
{
    [Serializable]
    public class WizardActionUrl
    {
        public WizardActionUrl() { }
        public WizardActionUrl(string action, string controller, string area)
        {
            Area = area;
            Controller = controller;
            Action = action;
        }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                string.IsNullOrWhiteSpace(Area) ? null : "/" + Area,
                string.IsNullOrWhiteSpace(Controller) ? null : "/" + Controller,
                string.IsNullOrWhiteSpace(Action) ? null : "/" + Action);
        }
    }
}

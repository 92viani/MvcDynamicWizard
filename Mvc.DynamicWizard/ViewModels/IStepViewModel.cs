using System;
using System.Web.Script.Serialization;

namespace Mvc.DynamicWizard.ViewModels
{
    public interface IStepViewModel
    {

    }

    [Serializable]
    public class StepViewModel : IStepViewModel
    {
        private WizardActionUrl ActionUrl;

        public string StepId { get; private set; }
        public int Index { get; internal set; }
        public string Title { get; set; }
        public string Descriptions { get; set; }
        public string Icon { get; set; }
        public string Url { get { return ActionUrl?.ToString(); } }
        public bool IsFirst { get; set; }
        public StepViewModel PreviousStep { get; set; }
        public WizardViewModel Context { get; internal set; }

        public StepViewModel()
        {
            StepId = Guid.NewGuid().ToString("N");
            Icon = "fa fa-bookmark-o";

        }
        public StepViewModel(string title,string descriptions,string cssIcon = "fa fa-bookmark-o")
        {
            StepId = Guid.NewGuid().ToString("N");
            Title = title;
            Descriptions = descriptions;
            Icon = cssIcon;
        }

        public StepViewModel SetActionUrl(string action) { return SetActionUrl(action, null, null); }
        public StepViewModel SetActionUrl(string action, string controller) { return SetActionUrl(action, controller, null); }
        public StepViewModel SetActionUrl(string action, string controller, string area)
        {
            ActionUrl = new WizardActionUrl(action,controller,area);
            return this;
        }
        public WizardActionUrl GetActionUrl() => ActionUrl;

        public string ToJsonSerializer()
        {
            var serializer = new JavaScriptSerializer();
            var JsonData = serializer.Serialize(this);
            return JsonData;
        }

    }
}

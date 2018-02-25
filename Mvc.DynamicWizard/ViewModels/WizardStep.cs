namespace Mvc.DynamicWizard.ViewModels
{
    public sealed class WizardStep
    {
        public WizardStep(string name, string partialView, object model = null)
        {
            this.StepName = name;
            this.StepPartialView = partialView;
            this.StepModel = model;
        }

        public bool IsFirst { get; internal set; }

        public bool IsLast { get; internal set; }

        public WizardStep NextStep { get; internal set; }

        public WizardStep PreviousStep { get; internal set; }

        public object StepModel { get; set; }

        public string StepName { get; private set; }

        public string StepPartialView { get; private set; }

        public string StepSubTitle { get; set; }

        public string StepTitle { get; set; }

    }
}
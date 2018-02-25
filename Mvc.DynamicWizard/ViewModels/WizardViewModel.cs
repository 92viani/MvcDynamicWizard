namespace Mvc.DynamicWizard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public sealed class WizardViewModel
    {
        private int stepIndex { get; set; }
        private IList<StepViewModel> Steps { get; set; }

        public IList<WizardValidationResult> Errors { get; internal set; }
        public string WizardId { get; private set; }
        public int StepIndex { get { return stepIndex; } }

        public StepViewModel CurrentStep
        {
            get { return Steps[stepIndex]; }
            set { Steps[stepIndex] = value; }
        }

        public WizardViewModel(Guid? id = null)
        {
            WizardId = id.HasValue ? id.Value.ToString("N") : Guid.NewGuid().ToString("N");
            Steps = new List<StepViewModel>();
        }

        public void Initialize()
        {
            Steps = typeof(StepViewModel)
                .Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(StepViewModel).IsAssignableFrom(t))
                .Select(t => (StepViewModel)Activator.CreateInstance(t))
                .ToList();
        }

        public WizardViewModel AddWizardStep(StepViewModel step)
        {
            step.Context = this;
            step.Index = StepCount;
            if (Steps.Count == 0)
                step.IsFirst = true;
            Steps.Insert(step.Index, step);
            return this;
        }

        public List<StepViewModel> GetSteps() => Steps.ToList();

        internal void SetStepIndex(int index)
        {
            if (Steps.Count > index)
                stepIndex = index;
        }

        public StepViewModel NextStep()
        {
            var _currentStep = this.CurrentStep;
            SetStepIndex(stepIndex + 1);
            CurrentStep.PreviousStep = _currentStep;
            return CurrentStep;
        }

        public StepViewModel PreviousStep()
        {
            var _currentStep = this.CurrentStep;
            SetStepIndex(stepIndex - 1);
            CurrentStep.PreviousStep = _currentStep;
            return CurrentStep;
        }

        /// <summary>
        /// Gets the type of the wizard.
        /// </summary>
        /// <value>The type of the wizard so the custom modelbinding knows what kind of object there is posted to the server.
        /// Just return the Type of the class of your ViewModel like this: return GetType().ToString();
        /// </value>
        public string WizardType
        {
            get { return GetType().ToString(); }
        }

        public bool FinalStep { get { return (Steps?.Count == stepIndex); } }
        public int StepCount { get { return (Steps?.Count) ?? 0; } }

    }

}

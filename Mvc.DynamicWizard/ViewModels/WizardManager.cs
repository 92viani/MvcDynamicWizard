using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc.DynamicWizard.ViewModels
{
    public class WizardManager
    {
        private const string buttonBack = "back";
        private const string buttonConfirm = "confirm";
        private const string buttonNext = "next";
        private const string stay = "stay";

        public WizardManager()
        {
            this.Steps = new List<WizardStep>();
            this.ConfirmAction = "Confirm";
            this.IsLastStep = true;
            this.IsConfermed = false;
        }

        public WizardManager AddStep(WizardStep step)
        {
            if (this.Steps.Count == 0)
            {
                this.FirstStep = step;
                step.IsFirst = true;
                this.Steps.Add(step);
                this.CurrentStep = step;
                this.IsLastStep = true;
            }
            else
            {
                this.IsLastStep = false;
                WizardStep step2 = this.Steps.Last();
                step2.NextStep = step;
                step2.IsLast = false;
                step.PreviousStep = step2;
                this.Steps.Add(step);
            }
            this.LastStep = step;
            step.IsLast = true;
            return this;
        }

        public WizardStep Back()
        {
            if (this.Steps.Count > 0)
            {
                this.IsLastStep = false;
            }
            if (!this.CurrentStep.IsFirst)
            {
                this.CurrentStep = this.CurrentStep.PreviousStep;
            }
            return this.CurrentStep;
        }

        public WizardStep First()
        {
            if (this.Steps.Count > 0)
            {
                this.IsLastStep = false;
            }
            this.CurrentStep = this.FirstStep;
            return this.CurrentStep;
        }

        public WizardStep GetStep(string stepName)
        {
            foreach (WizardStep step in this.Steps)
            {
                if (step.StepName.Equals(stepName))
                {
                    return step;
                }
            }
            return this.CurrentStep;
        }

        public WizardStep GoTo(string stepName)
        {
            foreach (WizardStep step in this.Steps)
            {
                if (step.StepName.Equals(stepName))
                {
                    this.CurrentStep = step;
                    break;
                }
            }
            if (this.CurrentStep == this.LastStep)
            {
                this.IsLastStep = true;
            }
            return this.CurrentStep;
        }

        public WizardStep Last()
        {
            this.IsLastStep = true;
            this.CurrentStep = this.LastStep;
            return this.CurrentStep;
        }

        public WizardStep Next()
        {
            if (this.CurrentStep.IsLast)
            {
                this.IsLastStep = true;
                return this.CurrentStep;
            }
            this.CurrentStep = this.CurrentStep.NextStep;
            return this.CurrentStep;
        }

        public string ButtonBack =>
            "back";

        public string ButtonConfirm =>
            "confirm";

        public string ButtonNext =>
            "next";

        public string ConfirmAction { get; set; }

        public int Count =>
            this.Steps.Count;

        public WizardStep CurrentStep { get; private set; }

        protected WizardStep FirstStep { get; set; }

        public int Index =>
            (1 + this.Steps.IndexOf(this.CurrentStep));

        public string IndexCount =>
            (this.Index + " / " + this.Count);

        public bool IsConfermed { get; set; }

        public bool IsLastStep { get; private set; }

        protected WizardStep LastStep { get; set; }

        public string Stay =>
            "stay";

        protected List<WizardStep> Steps { get; private set; }

        public object WizardModel { get; set; }
        
    }

    public class WizardManager<T> : WizardManager
    {
        public new T WizardModel { get; set; }
    }
}

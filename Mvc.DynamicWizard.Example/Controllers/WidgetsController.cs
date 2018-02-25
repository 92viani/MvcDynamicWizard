using Mvc.DynamicWizard.Controllers;
using Mvc.DynamicWizard.Sample.Models;
using Mvc.DynamicWizard.ViewModels;
using System.Web.Mvc;
using System;

namespace Mvc.DynamicWizard.Sample.Controllers
{
    public class WidgetsController : WizardController
    {
        public ActionResult Wizard(WizardViewModel wizardModel)
        {
            return WizardView(wizardModel);
        }

        public ActionResult Help()
        {
            Models.Help model = TempData["WizardStep"] as Models.Help;
            return PartialView(model);
        }
    }

    public class ExampleWizardController : WizardController<PersonInfo>
    {
        public ExampleWizardController()
        {
            base.WizardKeyName = "myWizard";
            base.EnableSimpleMerge = true;
        }

        public ActionResult ConfirmWizard(PersonInfo e)
        {
            if (base.Wizard().IsConfermed)
            {
                return base.View(base.Wizard().WizardModel);
            }
            return base.RedirectToAction("ActionStep");
        }

        protected override WizardManager<PersonInfo> InitWizard()
        {
            WizardManager<PersonInfo> manager = new WizardManager<PersonInfo>
            {
                ConfirmAction = "ConfirmWizard"
            };
            WizardStep step = new WizardStep("1", "Step1")
            {
                StepTitle = "Insert employ data",
                StepSubTitle = "Insert Name and Surname"
            };
            manager.AddStep(step);
            WizardStep step2 = new WizardStep("2", "Step2")
            {
                StepTitle = "Insert employ data",
                StepSubTitle = "Insert Address"
            };
            manager.AddStep(step2);
            WizardStep step3 = new WizardStep("3", "Step3")
            {
                StepTitle = "Insert employ data",
                StepSubTitle = "Insert Company"
            };
            manager.AddStep(step3);
            PersonInfo employ = new PersonInfo();
            manager.WizardModel = employ;
            return manager;
        }

        public ActionResult Step1(PersonInfo e) =>
            base.PartialView(base.Wizard().WizardModel);

        public ActionResult Step2(PersonInfo e) =>
            base.PartialView(base.Wizard().WizardModel);

        public ActionResult Step3(PersonInfo e)
        {
            if (!string.IsNullOrEmpty(e.NationalNo))
            {
                base.Wizard().WizardModel.NationalNo = e.NationalNo.ToUpper();
            }
            return base.PartialView(base.Wizard().WizardModel);
        }

        public override bool ValidateWizardModel(PersonInfo model)
        {
            if ("1".Equals(base.Wizard().CurrentStep.StepName))
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    base.ModelState.AddModelError("nullval", "Enter a value for Name");
                    return false;
                }
                if (string.IsNullOrEmpty(model.Family))
                {
                    base.ModelState.AddModelError("nullval", "Enter a value for Surname");
                    return false;
                }
            }
            if ("2".Equals(base.Wizard().CurrentStep.StepName))
            {
                if (string.IsNullOrEmpty(model.NationalNo))
                {
                    base.ModelState.AddModelError("nullval", "Enter a value for Address");
                    return false;
                }
                if (string.IsNullOrEmpty(model.PhoneNmber))
                {
                    base.ModelState.AddModelError("nullval", "Enter a value for City");
                    return false;
                }
            }
            if ("3".Equals(base.Wizard().CurrentStep.StepName))
            {
                if (string.IsNullOrEmpty(model.Descriptions))
                {
                    base.ModelState.AddModelError("nullval", "Enter a value for telephone");
                    return false;
                }
                if (string.IsNullOrEmpty(model.Title))
                {
                    base.ModelState.AddModelError("nullval", "Enter a value for Company");
                    return false;
                }
                base.Wizard().IsConfermed = true;
            }
            return true;
        }
    }

    public class ExampleAutoWizardController : AutoWizardController
    {
        public ExampleAutoWizardController()
        {
            base.WizardKeyName = "myAutoWizard";
        }

        public ActionResult Confirm()
        {
            if (base.Wizard().IsConfermed)
            {
                return base.View(base.Wizard().WizardModel);
            }
            return base.RedirectToAction("AutoStep");
        }

        protected override WizardManager InitWizard()
        {
            WizardManager manager = new WizardManager
            {
                ConfirmAction = "Confirm"
            };
            manager
                .AddStep(new WizardStep("1", "About", new { Name = "Moammad" }))
                .AddStep(new WizardStep("2", "Index"));
            return InitWizard(manager);
        }

        public override bool ValidateWizardForm(FormCollection form)
        {
            if (base.Wizard().CurrentStep.IsLast)
            {
                base.Wizard().IsConfermed = true;
            }
            return true;
        }
    }
}
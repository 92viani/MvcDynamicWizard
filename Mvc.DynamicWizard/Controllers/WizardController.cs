namespace Mvc.DynamicWizard.Controllers
{
    using ModelBinders;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Web.Mvc;
    using ViewModels;
    using System.Reflection;
    using System;

    public abstract class WizardController : Controller
    {
        #region Properties
        protected WizardViewModel GetWizard(string WizardKey)
        {
            WizardViewModel manager = (WizardViewModel)base.Session[WizardKey];
            if (manager == null)
            {
                base.Session[WizardKey] = null;
            }
            return manager;
        }
        protected void UpdateWizard(WizardViewModel wizard)
        {
            base.Session[wizard.WizardId] = wizard;
        }
        #endregion

        public virtual ActionResult WizardView(WizardViewModel wizardModel)
        {
            UpdateWizard(wizardModel);
            return View(wizardModel);
        }

        [HttpPost]
        public virtual ActionResult UpdateWizardStep(string key,string stepDirection)
        {
            if (string.IsNullOrEmpty(key))
                return HttpNotFound();

            var state = GetWizard(key);
            RemoveValidationRulesFromOtherSteps(state);

            Validate(ModelState, state);
            ProcessToUpdate(state);
            UpdateWizard(state);
            TempData["WizardStep"] = state.CurrentStep;

            var routeData = new System.Web.Routing.RouteValueDictionary(state.CurrentStep);
            routeData.Add("Area", state.CurrentStep.GetActionUrl().Area);

            return RedirectToAction(
                state.CurrentStep.GetActionUrl().Action,
                state.CurrentStep.GetActionUrl().Controller, 
                new { Area = state.CurrentStep.GetActionUrl().Area });
        }

        [HttpPost]
        public virtual JsonResult PreviousWizardStep(string key, FormCollection form)
        {
            var state = GetWizard(key);
            ModelState.Clear();
            state.Errors = null;
            state.CurrentStep = this.FormCollectToObj(state.CurrentStep, form);
            state.PreviousStep();
            ProcessToPrevious(state);

            UpdateWizard(state);
            return Json(state.StepIndex);
        }

        [HttpPost]
        public virtual JsonResult NextWizardStep(string key, FormCollection form)
        {
            var state = GetWizard(key);
            RemoveValidationRulesFromOtherSteps(state);
            state.CurrentStep = this.FormCollectToObj(state.CurrentStep, form);

            if (Validate(ModelState, state))
            {
                state.Errors = null;
                state.NextStep();

                try
                {
                    ProcessToNext(state);
                }
                catch (ValidationException valEx)
                {
                    // Catch custom exceptions so decrease the stepindex
                    state.PreviousStep();

                    // Return the errors to the client
                    state.Errors = new List<WizardValidationResult>();
                    state.Errors.Add(new WizardValidationResult { MemberName = string.Empty, Message = valEx.ValidationResult.ErrorMessage });
                }
            }

            UpdateWizard(state);
            return Json(state.StepIndex);
        }

        private void RemoveValidationRulesFromOtherSteps(WizardViewModel model)
        {
            foreach (var validationRuleFromOtherStep in ModelState.Where(m => !m.Key.StartsWith("Step" + (model.StepIndex))).ToList())
            {
                ModelState.Remove(validationRuleFromOtherStep.Key);
            }
        }

        protected virtual void ProcessToUpdate(WizardViewModel model)
        {
        }

        protected virtual void ProcessToPrevious(WizardViewModel model)
        {
        }

        protected virtual void ProcessToNext(WizardViewModel model)
        {
        }

        protected virtual List<WizardValidationResult> ValidationRules(WizardViewModel baseModel)
        {
            return new List<WizardValidationResult>();
        }

        private bool Validate(ModelStateDictionary modelStateDict, WizardViewModel stepModel)
        {
            stepModel.Errors = new List<WizardValidationResult>();

            if (!modelStateDict.IsValid)
            {
                // deze foreach voegt alle attribute errors toe
                foreach (var modelState in modelStateDict)
                {
                    if (modelState.Value.Errors.Any())
                    {
                        stepModel.Errors.Add(new WizardValidationResult { MemberName = modelState.Key, Message = modelState.Value.Errors[0].ErrorMessage });
                    }
                }
            }

            return modelStateDict.IsValid;
        }

        private T FormCollectToObj<T>(FormCollection formData)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (PropertyInfo pro in temp.GetProperties())
            {
                var y = formData[pro.Name];

                if (y == null || y == string.Empty || y == "") continue;

                if (pro.PropertyType.IsGenericType && pro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    pro.SetValue(obj, Convert.ChangeType(y, Nullable.GetUnderlyingType(pro.PropertyType)));
                }
                else
                {
                    pro.SetValue(obj, Convert.ChangeType(y, pro.PropertyType));
                }

            }
            return obj;
        }
        private StepViewModel FormCollectToObj(StepViewModel temp,FormCollection formData)
        {
            foreach (PropertyInfo pro in temp.GetType().GetProperties())
            {
                var y = formData[pro.Name];

                if (y == null || y == string.Empty || y == "") continue;

                if (pro.PropertyType.IsGenericType && pro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    pro.SetValue(temp, Convert.ChangeType(y, Nullable.GetUnderlyingType(pro.PropertyType)));
                }
                else
                {
                    pro.SetValue(temp, Convert.ChangeType(y, pro.PropertyType));
                }

            }
            return temp;
        }
    }

    public abstract class WizardController<T> : Controller
    {
        public WizardController()
        {
            this.WizardKeyName = "wizard";
            this.EnableSimpleMerge = true;
        }

        public ActionResult ActionStep(T model, string next, string back, string complete)
        {
            if (back != null)
            {
                this.Wizard().Back();
            }
            else if (next != null)
            {
                if (this.ValidateWizardModel(model))
                {
                    this.Wizard().Next();
                }
            }
            else if ((complete != null) && this.ValidateWizardModel(model))
            {
                this.Wizard().Last();
            }
            if (((back == null) && (next == null)) && (complete == null))
            {
                this.Clean();
            }
            if (this.EnableSimpleMerge)
            {
                this.Merge(this.Wizard().WizardModel, model);
            }
            if (this.Wizard().IsLastStep)
            {
                return base.RedirectToAction(this.Wizard().ConfirmAction, model);
            }
            return base.View(model);
        }

        public ActionResult ActionStepPartial(T model, string next, string back, string complete)
        {
            if (back != null)
            {
                this.Wizard().Back();
            }
            else if (next != null)
            {
                if (this.ValidateWizardModel(model))
                {
                    this.Wizard().Next();
                }
            }
            else if ((complete != null) && this.ValidateWizardModel(model))
            {
                this.Wizard().Last();
            }
            if (((back == null) && (next == null)) && (complete == null))
            {
                this.Clean();
            }
            if (this.EnableSimpleMerge)
            {
                this.Merge(this.Wizard().WizardModel, model);
            }
            if (this.Wizard().IsLastStep)
            {
                return base.RedirectToAction(this.Wizard().ConfirmAction, model);
            }
            return base.PartialView(model);
        }

        public void Clean()
        {
            if (base.Session[this.WizardKeyName] != null)
            {
                WizardManager<T> manager = (WizardManager<T>)base.Session[this.WizardKeyName];
                if (manager.IsConfermed)
                {
                    if ((manager.WizardModel != null) && (manager.WizardModel is IDisposable))
                    {
                        ((IDisposable)manager.WizardModel).Dispose();
                    }
                    base.Session[this.WizardKeyName] = null;
                }
            }
        }

        private object CreateType(Type t, string value)
        {
            try
            {
                if (t == typeof(string))
                {
                    return value;
                }
                if (t == typeof(int))
                {
                    return int.Parse(value);
                }
                if (t == typeof(double))
                {
                    return double.Parse(value);
                }
                if (t == typeof(DateTime))
                {
                    return DateTime.Parse(value);
                }
                if (t == typeof(float))
                {
                    return float.Parse(value);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Obsolete("Use the ActionStep method and partial view")]
        public ActionResult ExecuteStep(T model, string next, string back, string complete)
        {
            if (back != null)
            {
                this.Wizard().Back();
            }
            else if (next != null)
            {
                if (this.ValidateWizardModel(model))
                {
                    this.Wizard().Next();
                }
            }
            else if ((complete != null) && this.ValidateWizardModel(model))
            {
                this.Wizard().Last();
            }
            if (((back == null) && (next == null)) && (complete == null))
            {
                this.Clean();
            }
            if (this.EnableSimpleMerge)
            {
                this.Merge(this.Wizard().WizardModel, model);
            }
            if (this.Wizard().IsLastStep)
            {
                this.Wizard().IsConfermed = true;
                return base.RedirectToAction(this.Wizard().ConfirmAction, model);
            }
            return base.RedirectToAction(this.Wizard().CurrentStep.StepPartialView, model);
        }

        public void ForceClean()
        {
            if (base.Session[this.WizardKeyName] != null)
            {
                WizardManager<T> manager = (WizardManager<T>)base.Session[this.WizardKeyName];
                if ((manager.WizardModel != null) && (manager.WizardModel is IDisposable))
                {
                    ((IDisposable)manager.WizardModel).Dispose();
                }
                base.Session[this.WizardKeyName] = null;
            }
        }

        protected abstract WizardManager<T> InitWizard();

        private void Merge(object model, FormCollection form)
        {
            foreach (string str in form.Keys)
            {
                PropertyInfo property = model.GetType().GetProperty(str);
                if (property != null)
                {
                    object obj2 = this.CreateType(property.PropertyType, form[str]);
                    if (obj2 != null)
                    {
                        property.GetSetMethod().Invoke(model, new object[] { obj2 });
                    }
                }
            }
        }

        private void Merge(T wizard, T model)
        {
            foreach (PropertyInfo info in model.GetType().GetProperties())
            {
                object obj2 = info.GetGetMethod().Invoke(model, null);
                if (obj2 != null)
                {
                    wizard.GetType().GetProperty(info.Name).GetSetMethod().Invoke(wizard, new object[] { obj2 });
                }
            }
            foreach (FieldInfo info3 in model.GetType().GetFields())
            {
                object obj3 = info3.GetValue(model);
                if (obj3 != null)
                {
                    wizard.GetType().GetField(info3.Name).SetValue(wizard, obj3);
                }
            }
        }

        public virtual bool ValidateWizardModel(T model)
        {
            if (this.Wizard().IsLastStep)
            {
                this.Wizard().IsConfermed = true;
            }
            return true;
        }

        protected WizardManager<T> Wizard()
        {
            WizardManager<T> manager = (WizardManager<T>)base.Session[this.WizardKeyName];
            if (manager == null)
            {
                manager = this.InitWizard();
                base.Session[this.WizardKeyName] = manager;
            }
            return manager;
        }

        public bool EnableSimpleMerge { get; set; }

        public string WizardKeyName { get; set; }
    }
}

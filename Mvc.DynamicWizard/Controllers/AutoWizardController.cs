namespace Mvc.DynamicWizard.Controllers
{
    using ViewModels;
    using System;
    using System.Reflection;
    using System.Web.Mvc;

    public abstract class AutoWizardController : Controller
    {
        public AutoWizardController()
        {
            this.WizardKeyName = "wizard";
        }

        public ActionResult AutoStep(FormCollection form)
        {
            bool flag = this.ValidateWizardForm(form);
            if (flag)
            {
                this.Merge(this.Wizard().CurrentStep.StepModel, form);
            }
            if (form["back"] != null)
            {
                this.Wizard().Back();
            }
            else if (form["next"] != null)
            {
                if (flag)
                {
                    this.Wizard().Next();
                }
            }
            else if ((form["complete"] != null) && flag)
            {
                this.Wizard().Last();
            }
            if (((form["back"] == null) && (form["next"] == null)) && (form["complete"] == null))
            {
                this.Clean();
            }
            if (this.Wizard().IsLastStep)
            {
                return base.RedirectToAction(this.Wizard().ConfirmAction);
            }
            return base.View(this.Wizard().WizardModel);
        }

        public void Clean()
        {
            if (base.Session[this.WizardKeyName] != null)
            {
                WizardManager manager = (WizardManager) base.Session[this.WizardKeyName];
                if (manager.IsConfermed)
                {
                    if ((manager.WizardModel != null) && (manager.WizardModel is IDisposable))
                    {
                        ((IDisposable) manager.WizardModel).Dispose();
                    }
                    base.Session[this.WizardKeyName] = null;
                }
            }
        }

        private object CreateType(Type t, string value)
        {
            try
            {
                if (value != null)
                {
                    if (t == typeof(string))
                    {
                        return value;
                    }
                    if (t == typeof(int))
                    {
                        return int.Parse(value);
                    }
                    if (t == typeof(long))
                    {
                        return long.Parse(value);
                    }
                    if (t == typeof(short))
                    {
                        return short.Parse(value);
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
                    if (t == typeof(decimal))
                    {
                        return decimal.Parse(value);
                    }
                    if (t == typeof(long))
                    {
                        return long.Parse(value);
                    }
                    if (t == typeof(byte))
                    {
                        return byte.Parse(value);
                    }
                    if (t == typeof(bool))
                    {
                        if (value.IndexOf(",") > 0)
                        {
                            value = value.Split(new char[] { ',' })[0];
                        }
                        return bool.Parse(value);
                    }
                    if (t.IsSubclassOf(typeof(Enum)))
                    {
                        return Enum.Parse(t, value);
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ForceClean()
        {
            if (base.Session[this.WizardKeyName] != null)
            {
                WizardManager manager = (WizardManager) base.Session[this.WizardKeyName];
                if ((manager.WizardModel != null) && (manager.WizardModel is IDisposable))
                {
                    ((IDisposable) manager.WizardModel).Dispose();
                }
                base.Session[this.WizardKeyName] = null;
            }
        }

        protected abstract WizardManager InitWizard();
        protected WizardManager InitWizard(WizardManager wizardModel)
        {
            return wizardModel;
        }
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

        public virtual bool ValidateWizardForm(FormCollection form) => 
            true;

        protected WizardManager Wizard()
        {
            WizardManager manager = (WizardManager) base.Session[this.WizardKeyName];
            if (manager == null)
            {
                manager = this.InitWizard();
                base.Session[this.WizardKeyName] = manager;
            }
            return manager;
        }

        public string WizardKeyName { get; set; }
    }
}


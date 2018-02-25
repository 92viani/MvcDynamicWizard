using Microsoft.Web.Mvc;
using Mvc.DynamicWizard.Sample.Models;
using Mvc.DynamicWizard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc.DynamicWizard.Example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpPost]
        public ActionResult About(FormCollection formData)
        {
            Dictionary<string, object> form = new Dictionary<string, object>();
            formData.CopyTo(form);
            object test = FormCollectToObj<PersonInfo>(formData);
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.PersonWizard =
                new WizardViewModel()
                .AddWizardStep(new Help("راهنما", null).SetActionUrl("Help", "Widgets"))
                .AddWizardStep(new PersonInfo("اطلاعات پرسنلی", null).SetActionUrl("Info", "Personnel"))
                .AddWizardStep(new PersonInfo("1اطلاعات پرسنلی", null).SetActionUrl("Info", "Personnel"))
                .AddWizardStep(new PersonInfo("2اطلاعات پرسنلی", null).SetActionUrl("Info", "Personnel"));

            return View();
        }

        private T FormCollectToObj<T>(FormCollection dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (System.Reflection.PropertyInfo pro in temp.GetProperties())
            {
                var y = dr[pro.Name];

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
    }
}
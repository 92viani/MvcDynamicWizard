using Mvc.DynamicWizard.Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc.DynamicWizard.Sample.Controllers
{
    public class PersonnelController : Controller
    {
        [HttpGet]
        public ActionResult Info()
        {
            PersonInfo model = TempData["WizardStep"] as PersonInfo;
            return PartialView();
        }
        [HttpPost]
        public ActionResult Info(PersonInfo model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Error=1 }, JsonRequestBehavior.AllowGet);
            }
            return PartialView(model);
        }
    }
}
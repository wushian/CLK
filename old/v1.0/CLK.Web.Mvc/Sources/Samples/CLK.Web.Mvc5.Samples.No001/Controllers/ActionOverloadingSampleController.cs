using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLK.Web.Mvc.Samples.No001.Controllers
{
    public class ActionOverloadingSampleController : Controller
    {
        [Overload]
        public ActionResult Test()
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test()」";

            // Return
            return View();
        }

        [Overload]
        public ActionResult Test(string arg01)
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test(string arg01)」";
                        
            // Return
            return View();
        }

        [Overload]
        public ActionResult Test(string arg01, string arg02)
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test(string arg01, string arg02)」";

            // Return
            return View();
        }

        [Overload]
        public ActionResult Test(string arg01, string arg02, string arg03)
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test(string arg01, string arg02, string arg03)」";

            // Return
            return View();
        }
    }
}

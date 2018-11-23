using CLK.Web.Mvc;
using CLK.Web.Mvc.Samples.No001.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLK.Web.Mvc.Samples.No001.Controllers
{
    public class ModelConverterSampleController : Controller
    {
        [Overload]
        public ActionResult Test()
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test()」";
            this.ViewBag.Id = null;
            this.ViewBag.Birthday = null;
            this.ViewBag.GenderType = null;

            // Return
            return View();
        }

        [Overload]
        public ActionResult Test(string id, DateTime birthday, [GenderType]GenderType? genderType)
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test(string id, DateTime birthday, [GenderType]GenderType? genderType)」";
            this.ViewBag.Id = id;
            this.ViewBag.Birthday = birthday;
            this.ViewBag.GenderType = genderType.HasValue == true ? GenderTypeConverter.Current.Serialize(genderType.Value) : null;

            // Return
            return View();
        }

        [Overload]
        public ActionResult Test(EmployeeModel employee)
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test(EmployeeModel employee)」";
            this.ViewBag.Id = employee.Id;
            this.ViewBag.Birthday = employee.Birthday;
            this.ViewBag.GenderType = GenderTypeConverter.Current.Serialize(employee.GenderType);

            // Return
            return View();
        }
    }
}

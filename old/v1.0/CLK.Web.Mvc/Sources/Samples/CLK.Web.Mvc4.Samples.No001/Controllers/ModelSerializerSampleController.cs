using CLK.Web.Mvc;
using CLK.Web.Mvc.Samples.No001.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLK.Web.Mvc.Samples.No001.Controllers
{
    public class ModelSerializerSampleController : Controller
    {
        [Overload]
        public ActionResult Test()
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test()」";
            this.ViewBag.Result = null;

            // Return
            return View();
        }

        [Overload]
        public ActionResult Test(string id, DateTime birthday, [GenderType]GenderType? genderType)
        {
            // Employee
            var employee = new EmployeeModel();
            employee.Id = id;
            employee.Birthday = birthday;
            employee.GenderType = genderType;

            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test(string id, DateTime birthday, [GenderType]GenderType? genderType)」";
            this.ViewBag.Result = ModelSerializer.Serialize(employee);

            // Return
            return View();
        }

        [Overload]
        public ActionResult Test(EmployeeModel employee)
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test(EmployeeModel employee)」";
            this.ViewBag.Result = ModelSerializer.Serialize(employee, "employee");

            // Return
            return View();
        }

        [Overload]
        public ActionResult Test(OrderModel order)
        {
            // Result
            this.ViewBag.TriggerAction = @"Trigger Action：「Test(OrderModel order)」";
            this.ViewBag.Result = ModelSerializer.Serialize(order, "order");

            // Return
            return View();
        }
    }
}

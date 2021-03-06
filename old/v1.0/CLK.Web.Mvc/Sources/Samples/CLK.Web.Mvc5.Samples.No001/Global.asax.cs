﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CLK.Web.Mvc.Samples.No001
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Area
            AreaRegistration.RegisterAllAreas();

            // Config
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // DefaultBinder
            System.Web.Mvc.ModelBinders.Binders.DefaultBinder = new CLK.Web.Mvc.DefaultModelBinder();
        }
    }
}

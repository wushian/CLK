﻿using System.Web;
using System.Web.Mvc;

namespace CLK.Web.Mvc.Samples.No001
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLK.Web.Mvc.Samples.No001.Models
{
    public class EmployeeModel
    {
        // Properties
        public string Id { get; set; }

        public DateTime Birthday { get; set; }

        [GenderType]
        public GenderType? GenderType { get; set; }        
    }
}
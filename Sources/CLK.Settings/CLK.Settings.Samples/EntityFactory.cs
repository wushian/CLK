using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Settings.Samples
{
    public static class EntityFactory
    {
        // Methods
        public static Entity Create()
        {
            // Create
            Entity entity = new Entity();
            entity.Property001 = CLK.Settings.SettingContext.Current.AppSettings["AppSetting001"];
            entity.Property002 = CLK.Settings.SettingContext.Current.ConnectionStrings["ConnectionString001"];

            // Return
            return entity;
        }
    }
}

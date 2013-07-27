using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Settings;

namespace CLK.Reflection.Samples
{
    public class EntityBuilder : ReflectBuilder
    {
        // Methods
        protected override object CreateEntity(IReflectContext reflectContext, SettingContext settingContext)
        {
            #region Contracts

            if (reflectContext == null) throw new ArgumentNullException();
            if (settingContext == null) throw new ArgumentNullException();

            #endregion

            // Create
            Entity entity = new Entity();
            entity.Property001 = this.GetParameterValue("Property001");
            entity.Property002 = this.GetParameterValue("Property002");

            // Return
            return entity;
        }
    }
}

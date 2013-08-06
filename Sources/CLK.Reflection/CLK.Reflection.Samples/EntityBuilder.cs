using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Settings;

namespace CLK.Reflection.Samples
{
    public class EntityBuilder : ReflectBuilder
    {
        // Properties
        public string Property001
        {
            get { return this.GetParameterValue("Property001"); }
            set { this.SetParameterValue("Property001", value); }
        }

        public string Property002
        {
            get { return this.GetParameterValue("Property002"); }
            set { this.SetParameterValue("Property002", value); }
        }


        // Methods
        protected override object CreateEntity(IReflectContext reflectContext, SettingContext settingContext)
        {
            #region Contracts

            if (reflectContext == null) throw new ArgumentNullException();
            if (settingContext == null) throw new ArgumentNullException();

            #endregion

            // Create
            Entity entity = new Entity();
            entity.Property001 = this.Property001;
            entity.Property002 = this.Property002;

            // Return
            return entity;
        }
    }
}

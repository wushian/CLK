using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Configuration.Samples
{
    public sealed class EntityElementCollection : ConfigurationElementCollection
    {
        // Constructors
        public EntityElementCollection() { }


        // Properties
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }


        // Methods  
        protected override ConfigurationElement CreateNewElement()
        {
            // Base
            return new EntityElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            #region Contracts

            if (element == null) throw new ArgumentNullException();

            #endregion

            // Base
            return ((EntityElement)element).Name;
        }


        public void Add(EntityElement element)
        {
            #region Contracts

            if (element == null) throw new ArgumentNullException();

            #endregion

            // Base
            this.BaseAdd(element);
        }

        public void Remove(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // Base
            this.BaseRemove(name);
        }

        public bool Contains(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // Base
            if (this.BaseGet(name) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public EntityElement GetByName(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentNullException();

            #endregion

            // Base
            return (EntityElement)this.BaseGet(name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.ComponentModel.Exploring
{
    public abstract class ComponentHost<TComponent>
       where TComponent : Component
    {
        // Methods
        internal protected abstract void Start();

        internal protected abstract void Stop();


        // Events
        internal event Action<TComponent> ComponentArrived;
        protected void OnComponentArrived(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            var handler = this.ComponentArrived;
            if (handler != null)
            {
                handler(component);
            }
        }

        internal event Action<TComponent> ComponentDeparted;
        protected void OnComponentDeparted(TComponent component)
        {
            #region Contracts

            if (component == null) throw new ArgumentNullException();

            #endregion

            var handler = this.ComponentDeparted;
            if (handler != null)
            {
                handler(component);
            }
        }
    }
}

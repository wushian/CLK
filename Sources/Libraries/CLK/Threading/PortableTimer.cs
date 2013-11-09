using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Threading
{
    public abstract class PortableTimer
    {
        // Constructors
        public PortableTimer(int interval)
        {
            // Arguments
            this.Interval = interval;
        }


        // Properties  
        public int Interval { get; private set; }


        // Methods
        public abstract void Start();

        public abstract void Stop();


        // Events
        public event EventHandler Ticked;
        protected void OnTicked()
        {
            var handler = this.Ticked;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CLK.Threading
{
    public sealed class CommonTimer : PortableTimer
    {
        // Fields
        private System.Threading.Timer _operateTimer = null;
        

        // Constructors
        public CommonTimer(int interval) : base(interval) { }


        // Methods
        public override void Start()
        {
            // Timer
            _operateTimer = new Timer(this.Timer_Ticked, null, 0, this.Interval);
        }

        public override void Stop()
        {
            // Timer
            if (_operateTimer != null)
            {
                _operateTimer.Dispose();
                _operateTimer = null;
            }
        }


        // Handlers
        private void Timer_Ticked(Object stateInfo)
        {
            // Notify
            this.OnTicked();
        }
    }
}

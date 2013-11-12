using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public sealed class MulitDeviceCommandPipeline : DeviceCommandPipeline
    {
        // Methods
        protected override void Attach(DeviceCommandTask commandTask)
        {
            #region Contracts

            if (commandTask == null) throw new ArgumentNullException();

            #endregion

            // Execute
            this.Execute(commandTask);
        }

        protected override void Detach(DeviceCommandTask commandTask)
        {
            #region Contracts

            if (commandTask == null) throw new ArgumentNullException();

            #endregion

            // Nothing

        }
    }
}

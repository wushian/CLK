using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public sealed class SingleDeviceCommandPipeline : DeviceCommandPipeline
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly List<DeviceCommandTask> _commandTaskCollection = new List<DeviceCommandTask>();

        private DeviceCommandTask _currentCommandTask = null;


        // Methods
        protected override void Attach(DeviceCommandTask commandTask)
        {
            #region Contracts

            if (commandTask == null) throw new ArgumentNullException();

            #endregion

            // Attach
            lock (_syncRoot)
            {
                // Require
                if (_commandTaskCollection.Contains(commandTask) == true) return;

                // Add
                _commandTaskCollection.Add(commandTask);
            }

            // Execute
            this.Execute();
        }

        protected override void Detach(DeviceCommandTask commandTask)
        {
            #region Contracts

            if (commandTask == null) throw new ArgumentNullException();

            #endregion

            // Detach
            lock (_syncRoot)
            {
                // Require
                if (_commandTaskCollection.Contains(commandTask) == false) return;

                // Remove                
                _commandTaskCollection.Remove(commandTask);

                // Current
                if (_currentCommandTask == commandTask) _currentCommandTask = null;
            }

            // Execute
            this.Execute();
        }

        private void Execute()
        {
            // Result
            DeviceCommandTask commandTask = null;

            // Search 
            lock (_syncRoot)
            {
                // Current
                if (_currentCommandTask != null) return;
                if (_commandTaskCollection.Count <= 0) return;

                // Current                
                _currentCommandTask = _commandTaskCollection[0];
                commandTask = _commandTaskCollection[0];
            }

            // Execute
            if (commandTask != null) this.Execute(commandTask);
        }
    }
}

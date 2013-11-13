using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public abstract class DeviceCommandPipeline
    {
        // Constructors
        public DeviceCommandPipeline() { }


        // Methods
        internal void Post(DeviceCommandTask commandTask)
        {
            #region Contracts

            if (commandTask == null) throw new ArgumentException();

            #endregion

            // Events
            commandTask.ExecuteCommandBegan += this.CommandTask_ExecuteCommandBegan;
            commandTask.ExecuteCommandEnded += this.CommandTask_ExecuteCommandEnded;

            // Attach
            this.Attach(commandTask);
        }        
                
        protected void Execute(DeviceCommandTask commandTask)
        {
            #region Contracts

            if (commandTask == null) throw new ArgumentException();

            #endregion

            // Execute
            commandTask.ExecuteCommandAsync();
        }

        protected abstract void Attach(DeviceCommandTask commandTask);

        protected abstract void Detach(DeviceCommandTask commandTask);


        // Handlers
        private void CommandTask_ExecuteCommandBegan(DeviceCommandTask commandTask)
        {
            #region Contracts

            if (commandTask == null) throw new ArgumentException();

            #endregion

            // Nothing

        }

        private void CommandTask_ExecuteCommandEnded(DeviceCommandTask commandTask)
        {
            #region Contracts

            if (commandTask == null) throw new ArgumentException();

            #endregion

            // Events
            commandTask.ExecuteCommandBegan -= this.CommandTask_ExecuteCommandBegan;
            commandTask.ExecuteCommandEnded -= this.CommandTask_ExecuteCommandEnded;

            // Detach
            this.Detach(commandTask);
        }
    }
}

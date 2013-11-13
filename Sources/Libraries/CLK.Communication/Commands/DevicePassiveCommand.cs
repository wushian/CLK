using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Threading;

namespace CLK.Communication
{
    public class DevicePassiveCommand<TDeviceAddress, TRequest, TResponse, TStrategy> : DeviceCommand<TDeviceAddress, TRequest, TResponse>
        where TDeviceAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
        where TStrategy : class, IDevicePassiveCommandStrategy<TDeviceAddress, TRequest, TResponse>
    {
        // Fields
        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private TStrategy _commandStrategy = null;


        // Constructors
        public DevicePassiveCommand(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, DeviceCommandPipeline commandPipeline) : base(localDeviceAddress, remoteDeviceAddress, commandPipeline) { }

        internal override void Initialize(IDeviceCommandStrategy<TDeviceAddress> commandStrategy)
        {
            #region Contracts

            if (commandStrategy == null) throw new ArgumentNullException();

            #endregion

            // Require
            var currentCommandStrategy = commandStrategy as TStrategy;
            if (currentCommandStrategy == null) return;

            // Strategy
            _commandStrategy = currentCommandStrategy;

            // Base
            base.Initialize(commandStrategy);
        }
        

        // Methods
        internal override void Start()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Start
            try
            {
                // Require
                if (_commandStrategy == null) throw new InvalidOperationException();

                // Strategy
                _commandStrategy.ExecuteArrived += this.CommandStrategy_ExecuteArrived;

                // Base
                base.Start();
            }
            finally
            {
                // ExitStartLock
                _operateLock.ExitStartLock();
            }
        }
                
        internal override void Stop()
        {
            // EnterStopLock
            if (_operateLock.EnterStopLock() == false) return;

            // Stop
            try
            {
                // Require
                if (_commandStrategy == null) throw new InvalidOperationException();

                // Base
                base.Stop();

                // Strategy
                _commandStrategy.ExecuteArrived -= this.CommandStrategy_ExecuteArrived;
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }
        }

        public void ApplyExecuteCommandArrived(Guid taskId, TResponse response)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (response == null) throw new ArgumentNullException();

            #endregion
            
            // End
            this.EndExecute(taskId, response);
        }

        public void ApplyExecuteCommandArrived(Guid taskId, Exception error)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // End
            this.EndExecute(taskId, error);
        }


        // Handlers
        private void CommandStrategy_ExecuteArrived(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.LocalDeviceAddress.EqualAddress(localDeviceAddress) == false) return;
            if (this.RemoteDeviceAddress.EqualAddress(remoteDeviceAddress) == false) return;

            // Begin
            this.BeginExecute(taskId, request);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Threading;

namespace CLK.Communication
{
    public class DevicePassiveCommand<TAddress, TRequest, TResponse, TStrategy> : DeviceCommand<TAddress, TRequest, TResponse>
        where TAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
        where TStrategy : class, IDevicePassiveCommandStrategy<TAddress, TRequest, TResponse>
    {
        // Fields
        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private TStrategy _commandStrategy = null;


        // Constructors
        public DevicePassiveCommand(DeviceCommandPipeline commandPipeline) : base(commandPipeline) { }

        internal override void Initialize(IDeviceCommandStrategy<TAddress> commandStrategy)
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
            base.Initialize(_commandStrategy);
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

                // Base
                base.Start();

                // Strategy
                _commandStrategy.ExecuteArrived += this.CommandStrategy_ExecuteArrived;                
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
                                
                // Strategy
                _commandStrategy.ExecuteArrived -= this.CommandStrategy_ExecuteArrived;

                // Base
                base.Stop();
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
        private void CommandStrategy_ExecuteArrived(Guid taskId, TAddress localAddress, TAddress remoteAddress, TRequest request)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.LocalAddress.EqualAddress(localAddress) == false) return;
            if (this.RemoteAddress.EqualAddress(remoteAddress) == false) return;

            // Begin
            this.BeginExecute(taskId, request);
        }
    }
}

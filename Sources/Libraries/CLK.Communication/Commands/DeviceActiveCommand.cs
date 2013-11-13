using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Threading;
using System.Threading;

namespace CLK.Communication
{
    public class DeviceActiveCommand<TDeviceAddress, TRequest, TResponse, TStrategy> : DeviceCommand<TDeviceAddress, TRequest, TResponse>
        where TDeviceAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
        where TStrategy : class, IDeviceActiveCommandStrategy<TDeviceAddress, TRequest, TResponse>
    {
        // Fields
        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private TStrategy _commandStrategy = null;


        // Constructors
        public DeviceActiveCommand(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, DeviceCommandPipeline commandPipeline) : base(localDeviceAddress, remoteDeviceAddress, commandPipeline) { }

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
                _commandStrategy.ExecuteSucceedCompleted += this.CommandStrategy_ExecuteSucceedCompleted;
                _commandStrategy.ExecuteFailCompleted += this.CommandStrategy_ExecuteFailCompleted;

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
                _commandStrategy.ExecuteSucceedCompleted -= this.CommandStrategy_ExecuteSucceedCompleted;
                _commandStrategy.ExecuteFailCompleted -= this.CommandStrategy_ExecuteFailCompleted;
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }
        }
        

        public void ExecuteCommandAsync(Guid taskId, TRequest request)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Begin
            this.BeginExecute(taskId, request);
        }

        public TResponse ExecuteCommand(TRequest request)
        {
            #region Contracts

            if (request == null) throw new ArgumentNullException();

            #endregion

            // Variables
            Guid taskId = Guid.NewGuid();            
            EventHandler<ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>> eventHandler = null;
            ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> resultEventArgs = null;
            ManualResetEvent executeEvent = new ManualResetEvent(false);

            // Execute
            eventHandler = delegate(object sender, ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> eventArgs)
            {
                // Require
                if (eventArgs.TaskId != taskId) return;

                // Events
                this.ExecuteCommandCompleted -= eventHandler;

                // Result
                resultEventArgs = eventArgs;

                // Set
                executeEvent.Set();
            };
            this.ExecuteCommandCompleted += eventHandler;
            this.ExecuteCommandAsync(taskId, request);
            executeEvent.WaitOne();

            // Return
            if (resultEventArgs.Cancelled == true)
            {
                throw resultEventArgs.Error;
            }
            return resultEventArgs.Response;
        }


        // Handlers
        private void CommandStrategy_ExecuteSucceedCompleted(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request, TResponse response)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();
            if (response == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.LocalDeviceAddress.EqualAddress(localDeviceAddress) == false) return;
            if (this.RemoteDeviceAddress.EqualAddress(remoteDeviceAddress) == false) return;

            // End
            this.EndExecute(taskId, response);
        }

        private void CommandStrategy_ExecuteFailCompleted(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request, Exception error)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.LocalDeviceAddress.EqualAddress(localDeviceAddress) == false) return;
            if (this.RemoteDeviceAddress.EqualAddress(remoteDeviceAddress) == false) return;

            // End
            this.EndExecute(taskId, error);
        }
    }
}

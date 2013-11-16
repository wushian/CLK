using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Threading;
using System.Threading;

namespace CLK.Communication
{
    public class DeviceActiveCommand<TAddress, TRequest, TResponse, TStrategy> : DeviceCommand<TAddress, TRequest, TResponse>
        where TAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
        where TStrategy : class, IDeviceActiveCommandStrategy<TAddress, TRequest, TResponse>
    {
        // Fields
        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private TStrategy _commandStrategy = null;


        // Constructors
        public DeviceActiveCommand(DeviceCommandPipeline commandPipeline) : base(commandPipeline) { }

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
                _commandStrategy.ExecuteSucceedCompleted += this.CommandStrategy_ExecuteSucceedCompleted;
                _commandStrategy.ExecuteFailCompleted += this.CommandStrategy_ExecuteFailCompleted;                
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
                _commandStrategy.ExecuteSucceedCompleted -= this.CommandStrategy_ExecuteSucceedCompleted;
                _commandStrategy.ExecuteFailCompleted -= this.CommandStrategy_ExecuteFailCompleted;

                // Base
                base.Stop();
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
            EventHandler<ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse>> eventHandler = null;
            ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse> resultEventArgs = null;
            ManualResetEvent executeEvent = new ManualResetEvent(false);

            // Execute
            eventHandler = delegate(object sender, ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse> eventArgs)
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
        private void CommandStrategy_ExecuteSucceedCompleted(Guid taskId, TAddress localAddress, TAddress remoteAddress, TRequest request, TResponse response)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();
            if (response == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.LocalAddress.EqualAddress(localAddress) == false) return;
            if (this.RemoteAddress.EqualAddress(remoteAddress) == false) return;

            // End
            this.EndExecute(taskId, response);
        }

        private void CommandStrategy_ExecuteFailCompleted(Guid taskId, TAddress localAddress, TAddress remoteAddress, TRequest request, Exception error)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Require
            if (this.LocalAddress.EqualAddress(localAddress) == false) return;
            if (this.RemoteAddress.EqualAddress(remoteAddress) == false) return;

            // End
            this.EndExecute(taskId, error);
        }
    }
}

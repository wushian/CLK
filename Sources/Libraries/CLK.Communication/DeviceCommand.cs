using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CLK.Diagnostics;

namespace CLK.Communication
{
    public abstract class DeviceCommand<TAddress>
        where TAddress : DeviceAddress
    {
        // Constructors
        internal DeviceCommand() { }

        internal abstract void Initialize(TAddress localAddress, TAddress remoteAddress);

        internal abstract void Initialize(IDeviceCommandStrategy<TAddress> commandStrategy);


        // Methods        
        internal abstract void ApplyTimeTicked(long nowTicks);

        internal abstract void Start();

        internal abstract void Stop();
    }

    public abstract class DeviceCommand<TAddress, TRequest, TResponse> : DeviceCommand<TAddress>
        where TAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Constants
        private static Exception _defaultTimeoutException = null;
        private static Exception DefaultTimeoutException
        {
            get
            {
                if (_defaultTimeoutException == null)
                {
                    _defaultTimeoutException = new TimeoutException();
                }
                return _defaultTimeoutException;
            }
        }


        // Fields
        private readonly object _syncRoot = new object();

        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();

        private readonly List<DeviceCommandTask<TAddress, TRequest, TResponse>> _commandTaskCollection = new List<DeviceCommandTask<TAddress, TRequest, TResponse>>();
                
        private readonly DeviceCommandPipeline _commandPipeline = null;

        private TAddress _localAddress = null;

        private TAddress _remoteAddress = null;

        private IDeviceCommandStrategy<TAddress, TRequest, TResponse> _commandStrategy = null;


        // Constructors
        internal DeviceCommand(DeviceCommandPipeline commandPipeline)
        {
            #region Contracts

            if (commandPipeline == null) throw new ArgumentNullException();

            #endregion

            // Arguments            
            _commandPipeline = commandPipeline;
        }

        internal override void Initialize(TAddress localAddress, TAddress remoteAddress)
        {
            #region Contracts

            if (localAddress == null) throw new ArgumentNullException();
            if (remoteAddress == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _localAddress = localAddress;
            _remoteAddress = remoteAddress;
        }

        internal override void Initialize(IDeviceCommandStrategy<TAddress> commandStrategy)
        {
            #region Contracts

            if (commandStrategy == null) throw new ArgumentNullException();

            #endregion

            // Require
            var currentCommandStrategy = commandStrategy as IDeviceCommandStrategy<TAddress, TRequest, TResponse>;
            if (currentCommandStrategy == null) throw new InvalidOperationException();

            // Strategy
            _commandStrategy = currentCommandStrategy;
        }

        
        // Properties
        internal TAddress LocalAddress { get { return _localAddress; } }

        internal TAddress RemoteAddress { get { return _remoteAddress; } }


        // Methods       
        internal override void ApplyTimeTicked(long nowTicks)
        {
            // Timeout 
            Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> timeoutPredicate = delegate(DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask)
            {
                if (existCommandTask.ExpireTime.Ticks <= nowTicks)
                {
                    if (existCommandTask.RetryCount <= 0)
                    {
                        return true;
                    }
                }
                return false;
            };
            this.EndExecute(timeoutPredicate, DeviceCommand<TAddress, TRequest, TResponse>.DefaultTimeoutException);

            // Retry
            Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> retryPredicate = delegate(DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask)
            {
                if (existCommandTask.ExpireTime.Ticks <= nowTicks)
                {
                    return true;
                }
                return false;
            };
            this.RetryAllTask(retryPredicate);
        }

        internal override void Start()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Start
            try
            {
                // Require
                if (_localAddress == null) throw new InvalidOperationException();
                if (_remoteAddress == null) throw new InvalidOperationException();
                if (_commandStrategy == null) throw new InvalidOperationException();

                // Predicate 
                Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask)
                {
                    return true;
                };

                // EndExecute
                this.EndExecute(predicate, new DisconnectException());
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
                // Predicate 
                Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask)
                {
                    return true;
                };

                // EndExecute
                this.EndExecute(predicate, new DisconnectException());
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }
        }


        internal DeviceCommandTask<TAddress, TRequest, TResponse> CreateTask(Guid taskId, TRequest request)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Create
            var commandTask = new DeviceCommandTask<TAddress, TRequest, TResponse>(taskId, _localAddress, _remoteAddress, request, _commandStrategy.ExpireMillisecond, _commandStrategy.RetryCount);

            // Attach
            lock (_syncRoot)
            {
                // Add
                _commandTaskCollection.Add(commandTask);

                // Events
                commandTask.ExecuteCommandArrived += this.CommandTask_ExecuteCommandArrived;
                commandTask.ExecuteCommandCompleted += this.CommandTask_ExecuteCommandCompleted;
            }

            // Return
            return commandTask;
        }        

        private DeviceCommandTask<TAddress, TRequest, TResponse> DetachTask(Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            DeviceCommandTask<TAddress, TRequest, TResponse> commandTask = null;

            // Detach
            lock (_syncRoot)
            {
                // Search 
                foreach (DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask in _commandTaskCollection)
                {
                    if (predicate(existCommandTask) == true)
                    {
                        commandTask = existCommandTask;
                        break;
                    }
                }

                // Remove
                if (commandTask != null)
                {
                    // Remove
                    _commandTaskCollection.Remove(commandTask);

                    // Events
                    commandTask.ExecuteCommandArrived -= this.CommandTask_ExecuteCommandArrived;
                    commandTask.ExecuteCommandCompleted -= this.CommandTask_ExecuteCommandCompleted;
                }
            }

            // Return
            return commandTask;
        }

        private IEnumerable<DeviceCommandTask<TAddress, TRequest, TResponse>> DetachAllTask(Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<DeviceCommandTask<TAddress, TRequest, TResponse>> commandTaskCollection = new List<DeviceCommandTask<TAddress, TRequest, TResponse>>();

            // Detach
            lock (_syncRoot)
            {
                // Search 
                foreach (DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask in _commandTaskCollection)
                {
                    if (predicate(existCommandTask) == true)
                    {
                        commandTaskCollection.Add(existCommandTask);
                    }
                }

                // Remove
                foreach (DeviceCommandTask<TAddress, TRequest, TResponse> commandTask in commandTaskCollection)
                {
                    // Remove
                    _commandTaskCollection.Remove(commandTask);

                    // Events
                    commandTask.ExecuteCommandArrived -= this.CommandTask_ExecuteCommandArrived;
                    commandTask.ExecuteCommandCompleted -= this.CommandTask_ExecuteCommandCompleted;
                }
            }

            // Return
            return commandTaskCollection;
        }

        private IEnumerable<DeviceCommandTask<TAddress, TRequest, TResponse>> RetryAllTask(Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<DeviceCommandTask<TAddress, TRequest, TResponse>> commandTaskCollection = new List<DeviceCommandTask<TAddress, TRequest, TResponse>>();

            // Retry
            lock (_syncRoot)
            {
                // Search 
                foreach (DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask in _commandTaskCollection)
                {
                    if (predicate(existCommandTask) == true)
                    {
                        commandTaskCollection.Add(existCommandTask);
                    }
                }

                // Execute
                foreach (DeviceCommandTask<TAddress, TRequest, TResponse> commandTask in commandTaskCollection)
                {
                    commandTask.ExecuteCommandAsync();
                }
            }

            // Return
            return commandTaskCollection;
        }


        internal void BeginExecute(Guid taskId, TRequest request)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Create
            var commandTask = this.CreateTask(taskId, request);
            if (commandTask == null) throw new InvalidOperationException();

            // Post
            _commandPipeline.Post(commandTask);
        }

        internal void EndExecute(Guid taskId, TResponse response)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (response == null) throw new ArgumentNullException();

            #endregion

            // Predicate
            Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask)
            {
                if (existCommandTask.TaskId == taskId)
                {
                    return true;
                }
                return false;
            };

            // Detach     
            var commandTask = this.DetachTask(predicate);
            if (commandTask == null) return;
            
            // EndExecute
            commandTask.EndExecute(response);
        }

        internal void EndExecute(Guid taskId, Exception error)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Predicate
            Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TAddress, TRequest, TResponse> existCommandTask)
            {
                if (existCommandTask.TaskId == taskId)
                {
                    return true;
                }
                return false;
            };

            // Detach     
            var commandTask = this.DetachTask(predicate);
            if (commandTask == null) return;

            // EndExecute
            commandTask.EndExecute(error);
        }
        
        private void EndExecute(Func<DeviceCommandTask<TAddress, TRequest, TResponse>, bool> predicate, Exception error)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Detach 
            var commandTaskCollection = this.DetachAllTask(predicate);
            if (commandTaskCollection == null) return;

            // EndExecute
            foreach (DeviceCommandTask<TAddress, TRequest, TResponse> commandTask in commandTaskCollection)
            {
                commandTask.EndExecute(error);
            }
        }


        // Handlers
        private void CommandTask_ExecuteCommandArrived(object sender, ExecuteCommandArrivedEventArgs<TAddress, TRequest> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            // Execute
            try
            {
                // Require
                if (_operateLock.IsStarted == false) throw new DisconnectException();

                // Apply
                _commandStrategy.ApplyExecute(eventArgs);
            }
            catch (Exception error)
            {
                // End
                this.EndExecute(eventArgs.TaskId, error);
            }

            // Notify
            try
            {
                this.OnExecuteCommandArrived(eventArgs);
            }
            catch (Exception ex)
            {
                DebugContext.Current.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "OnExecuteCommandArrived", "Exception", ex.Message));
            }
        }

        private void CommandTask_ExecuteCommandCompleted(object sender, ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            // Execute
            try
            {
                // Require
                if (_operateLock.IsStarted == false) throw new DisconnectException();

                // Apply
                _commandStrategy.ApplyExecute(eventArgs);
            }
            catch (Exception ex)
            {
                eventArgs = new ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse>(eventArgs.TaskId, eventArgs.LocalAddress, eventArgs.RemoteAddress, eventArgs.Request, ex);
            }

            // Notify
            try
            {
                this.OnExecuteCommandCompleted(eventArgs);
            }
            catch (Exception ex)
            {
                DebugContext.Current.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "OnExecuteCommandCompleted", "Exception", ex.Message));
            }
        }


        // Events
        public event EventHandler<ExecuteCommandArrivedEventArgs<TAddress, TRequest>> ExecuteCommandArrived;
        private void OnExecuteCommandArrived(ExecuteCommandArrivedEventArgs<TAddress, TRequest> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            var handler = this.ExecuteCommandArrived;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public event EventHandler<ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse>> ExecuteCommandCompleted;
        private void OnExecuteCommandCompleted(ExecuteCommandCompletedEventArgs<TAddress, TRequest, TResponse> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            var handler = this.ExecuteCommandCompleted;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
    }
}

using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CLK.Communication
{
    public abstract class DeviceCommand
    {
        // Constructors
        internal DeviceCommand() { }


        // Methods        
        internal abstract void ApplyTimeTicked(long nowTicks);

        internal abstract void Start();

        internal abstract void Stop();
    }

    public abstract class DeviceCommand<TDeviceAddress, TRequest, TResponse> : DeviceCommand
        where TDeviceAddress : DeviceAddress
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

        private readonly List<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>> _commandTaskCollection = new List<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>>();

        private readonly TDeviceAddress _localDeviceAddress = null;

        private readonly TDeviceAddress _remoteDeviceAddress = null;

        private readonly DeviceCommandPipeline _commandPipeline = null;


        // Constructors
        public DeviceCommand(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, DeviceCommandPipeline commandPipeline)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (commandPipeline == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            _localDeviceAddress = localDeviceAddress;
            _remoteDeviceAddress = remoteDeviceAddress;
            _commandPipeline = commandPipeline;
        }


        // Properties
        protected abstract int RetryCount { get; }

        protected abstract int ExpireMillisecond { get; }


        // Methods       
        internal override void ApplyTimeTicked(long nowTicks)
        {
            // Timeout 
            Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> timeoutPredicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
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
            this.EndExecute(timeoutPredicate, DeviceCommand<TDeviceAddress, TRequest, TResponse>.DefaultTimeoutException);

            // Retry
            Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> retryPredicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
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
                // Predicate 
                Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
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
                Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
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


        protected DeviceCommandTask<TDeviceAddress, TRequest, TResponse> CreateTask(Guid taskId, TRequest request)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Create
            var commandTask = new DeviceCommandTask<TDeviceAddress, TRequest, TResponse>(taskId, _localDeviceAddress, _remoteDeviceAddress, request, this.ExpireMillisecond, this.RetryCount);

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

        private DeviceCommandTask<TDeviceAddress, TRequest, TResponse> DetachTask(Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            DeviceCommandTask<TDeviceAddress, TRequest, TResponse> commandTask = null;

            // Detach
            lock (_syncRoot)
            {
                // Search 
                foreach (DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask in _commandTaskCollection)
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

        private IEnumerable<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>> DetachAllTask(Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>> commandTaskCollection = new List<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>>();

            // Detach
            lock (_syncRoot)
            {
                // Search 
                foreach (DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask in _commandTaskCollection)
                {
                    if (predicate(existCommandTask) == true)
                    {
                        commandTaskCollection.Add(existCommandTask);
                    }
                }

                // Remove
                foreach (DeviceCommandTask<TDeviceAddress, TRequest, TResponse> commandTask in commandTaskCollection)
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

        private IEnumerable<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>> RetryAllTask(Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();

            #endregion

            // Result
            List<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>> commandTaskCollection = new List<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>>();

            // Retry
            lock (_syncRoot)
            {
                // Search 
                foreach (DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask in _commandTaskCollection)
                {
                    if (predicate(existCommandTask) == true)
                    {
                        commandTaskCollection.Add(existCommandTask);
                    }
                }

                // Execute
                foreach (DeviceCommandTask<TDeviceAddress, TRequest, TResponse> commandTask in commandTaskCollection)
                {
                    commandTask.ExecuteCommandAsync();
                }
            }

            // Return
            return commandTaskCollection;
        }


        protected void BeginExecute(Guid taskId, TRequest request)
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

        protected void EndExecute(Guid taskId, TResponse response)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (response == null) throw new ArgumentNullException();

            #endregion

            // Predicate
            Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
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

        protected void EndExecute(Guid taskId, Exception error)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Predicate
            Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
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
        
        protected void EndExecute(Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate, Exception error)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Detach 
            var commandTaskCollection = this.DetachAllTask(predicate);
            if (commandTaskCollection == null) return;

            // EndExecute
            foreach (DeviceCommandTask<TDeviceAddress, TRequest, TResponse> commandTask in commandTaskCollection)
            {
                commandTask.EndExecute(error);
            }
        }


        internal abstract void ApplyExecute(ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest> eventArgs);

        internal abstract void ApplyExecute(ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> eventArgs);


        // Handlers
        private void CommandTask_ExecuteCommandArrived(object sender, ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest> eventArgs)
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
                this.ApplyExecute(eventArgs);
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
                Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "OnExecuteCommandArrived", "Exception", ex.Message));
            }
        }

        private void CommandTask_ExecuteCommandCompleted(object sender, ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> eventArgs)
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
                this.ApplyExecute(eventArgs);
            }
            catch (Exception ex)
            {
                eventArgs = new ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>(eventArgs.TaskId, eventArgs.LocalDeviceAddress, eventArgs.RemoteDeviceAddress, eventArgs.Request, ex);
            }

            // Notify
            try
            {
                this.OnExecuteCommandCompleted(eventArgs);
            }
            catch (Exception ex)
            {
                Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "OnExecuteCommandCompleted", "Exception", ex.Message));
            }
        }


        // Events
        public event EventHandler<ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest>> ExecuteCommandArrived;
        private void OnExecuteCommandArrived(ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest> eventArgs)
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

        public event EventHandler<ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>> ExecuteCommandCompleted;
        private void OnExecuteCommandCompleted(ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> eventArgs)
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

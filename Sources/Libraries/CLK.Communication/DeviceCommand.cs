using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public abstract class DeviceCommand
    {
        // Constructors
        internal DeviceCommand() { }


        // Methods        
        internal protected abstract void ApplyTimeTicked(long nowTicks);

        internal protected abstract void Start();

        internal protected abstract void Stop();
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
        protected abstract int ExpireMillisecond { get; }


        // Methods       
        internal protected override void ApplyTimeTicked(long nowTicks)
        {
            // Predicate 
            Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
            {
                if (existCommandTask.ExpireTime.Ticks <= nowTicks)
                {
                    return true;
                }
                return false;
            };

            // EndExecute
            this.EndExecute(predicate, DeviceCommand<TDeviceAddress, TRequest, TResponse>.DefaultTimeoutException);
        }

        internal protected override void Start()
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

        internal protected override void Stop()
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
            var commandTask = new DeviceCommandTask<TDeviceAddress, TRequest, TResponse>(taskId, _localDeviceAddress, _remoteDeviceAddress, request, this.ExpireMillisecond);

            // Attach
            lock (_syncRoot)
            {
                // Add
                _commandTaskCollection.Add(commandTask);

                // Events
                commandTask.ExecuteCommandArrived += this.CommandTask_ExecuteCommandArrived;
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

            // Result
            DeviceCommandTask<TDeviceAddress, TRequest, TResponse> commandTask = null;

            // Detach          
            Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
            {
                if (existCommandTask.TaskId == taskId)
                {
                    return true;
                }
                return false;
            };
            commandTask = this.DetachTask(predicate);
            if (commandTask == null) return;

            // EndExecute
            try
            {
                // Create
                var eventArgs = new ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>(commandTask.TaskId, commandTask.LocalDeviceAddress, commandTask.RemoteDeviceAddress, commandTask.Request, response);

                // Apply
                this.ApplyExecute(eventArgs);

                // Notify
                this.OnExecuteCommandCompleted(eventArgs);

                // End
                commandTask.EndExecute(eventArgs);
            }
            catch (Exception ex)
            {
                Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "EndExecute", "Exception", ex.Message));
            }
        }

        protected void EndExecute(Guid taskId, Exception error)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Result
            DeviceCommandTask<TDeviceAddress, TRequest, TResponse> commandTask = null;

            // Detach          
            Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate = delegate(DeviceCommandTask<TDeviceAddress, TRequest, TResponse> existCommandTask)
            {
                if (existCommandTask.TaskId == taskId)
                {
                    return true;
                }
                return false;
            };
            commandTask = this.DetachTask(predicate);
            if (commandTask == null) return;

            // EndExecute
            try
            {
                // Create
                var eventArgs = new ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>(commandTask.TaskId, commandTask.LocalDeviceAddress, commandTask.RemoteDeviceAddress, commandTask.Request, error);

                // Apply
                this.ApplyExecute(eventArgs);

                // Notify
                this.OnExecuteCommandCompleted(eventArgs);

                // End
                commandTask.EndExecute(eventArgs);
            }
            catch (Exception ex)
            {
                Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "EndExecute", "Exception", ex.Message));
            }
        }

        private void EndExecute(Func<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>, bool> predicate, Exception error)
        {
            #region Contracts

            if (predicate == null) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Result
            IEnumerable<DeviceCommandTask<TDeviceAddress, TRequest, TResponse>> commandTaskCollection = null;

            // Detach 
            commandTaskCollection = this.DetachAllTask(predicate);
            if (commandTaskCollection == null) throw new InvalidOperationException();

            // EndExecute
            foreach (DeviceCommandTask<TDeviceAddress, TRequest, TResponse> commandTask in commandTaskCollection)
            {
                try
                {
                    // Create
                    var eventArgs = new ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>(commandTask.TaskId, commandTask.LocalDeviceAddress, commandTask.RemoteDeviceAddress, commandTask.Request, error);

                    // Apply
                    this.ApplyExecute(eventArgs);

                    // Notify
                    this.OnExecuteCommandCompleted(eventArgs);

                    // End
                    commandTask.EndExecute(eventArgs);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "EndExecute", "Exception", ex.Message));
                }
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

            // Notify
            try
            {
                this.OnExecuteCommandArrived(eventArgs);
            }
            catch (Exception ex)
            {
                Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "OnExecuteCommandArrived", "Exception", ex.Message));
            }

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

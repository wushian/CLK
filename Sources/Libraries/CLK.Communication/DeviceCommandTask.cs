using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public abstract class DeviceCommandTask
    {
        // Fields        
        private readonly int _expireMillisecond = int.MinValue;

        private int _retryCount = int.MinValue;        

        private DateTime _expireTime = DateTime.MaxValue;


        // Constructors
        internal DeviceCommandTask(int expireMillisecond, int retryCount) 
        {
            // Require            
            if (expireMillisecond <= 0) throw new InvalidOperationException();
            if (retryCount <= 0) throw new InvalidOperationException();

            // Arguments                       
            _expireMillisecond = expireMillisecond;
            _retryCount = retryCount;
        }


        // Properties         
        internal DateTime ExpireTime
        {
            get
            {
                lock (this)
                {
                    return _expireTime;
                }
            }
        }

        internal int RetryCount
        {
            get
            {
                lock (this)
                {
                    return _retryCount;
                }
            }
        }
             

        // Methods
        internal void ExecuteCommandAsync()
        {
            // Calculate
            lock (this)
            {
                // Require
                if (_retryCount <= 0) return;

                // ExpireTime
                _expireTime = DateTime.Now.AddMilliseconds(_expireMillisecond);

                // RetryCount                
                _retryCount--;                
            }

            // BeginExecute
            WaitCallback executeDelegate = delegate(object state)
            {
                try
                {
                    this.BeginExecute();
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Action:{0}, State:{1}, Message:{2}", "BeginExecute", "Exception", ex.Message));
                }
            };
            ThreadPool.QueueUserWorkItem(executeDelegate);
        }

        internal virtual void BeginExecute()
        {
            // Notify
            this.OnExecuteCommandBegan();
        }

        internal virtual void EndExecute()
        {
            // Notify
            this.OnExecuteCommandEnded();
        }


        // Events
        internal event Action<DeviceCommandTask> ExecuteCommandBegan;
        private void OnExecuteCommandBegan()
        {
            var handler = this.ExecuteCommandBegan;
            if (handler != null)
            {
                handler(this);
            }
        }

        internal event Action<DeviceCommandTask> ExecuteCommandEnded;
        private void OnExecuteCommandEnded()
        {
            var handler = this.ExecuteCommandEnded;
            if (handler != null)
            {
                handler(this);
            }
        }
    }

    public sealed class DeviceCommandTask<TDeviceAddress, TRequest, TResponse> : DeviceCommandTask
        where TDeviceAddress : DeviceAddress
        where TRequest : class
        where TResponse : class
    {
        // Constructors
        internal DeviceCommandTask(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request, int expireMillisecond, int retryCount)
            : base(expireMillisecond, retryCount)
        {
            #region Contracts

            if (taskId == Guid.Empty) throw new ArgumentException();
            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (request == null) throw new ArgumentNullException();

            #endregion

            // Arguments    
            this.TaskId = taskId;
            this.LocalDeviceAddress = localDeviceAddress;
            this.RemoteDeviceAddress = remoteDeviceAddress;
            this.Request = request;
        }


        // Properties
        public Guid TaskId { get; private set; }

        public TDeviceAddress LocalDeviceAddress { get; private set; }

        public TDeviceAddress RemoteDeviceAddress { get; private set; }

        public TRequest Request { get; private set; }   


        // Methods
        internal override void BeginExecute()
        {
            // Create
            var eventArgs = new ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest>(this.TaskId, this.LocalDeviceAddress, this.RemoteDeviceAddress, this.Request);

            // Base
            base.BeginExecute();     

            // Notify
            this.OnExecuteCommandArrived(eventArgs);                
        }

        internal void EndExecute(TResponse response)
        {
            #region Contracts

            if (response == null) throw new ArgumentNullException();

            #endregion

            // Create
            var eventArgs = new ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>(this.TaskId, this.LocalDeviceAddress, this.RemoteDeviceAddress, this.Request, response);
            
            // Notify
            this.OnExecuteCommandCompleted(eventArgs);

            // Base
            base.EndExecute();
        }

        internal void EndExecute(Exception error)
        {
            #region Contracts

            if (error == null) throw new ArgumentNullException();

            #endregion

            // Create
            var eventArgs = new ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>(this.TaskId, this.LocalDeviceAddress, this.RemoteDeviceAddress, this.Request, error);

            // Notify
            this.OnExecuteCommandCompleted(eventArgs);

            // Base
            base.EndExecute();
        }


        // Events
        internal event EventHandler<ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest>> ExecuteCommandArrived;
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

        internal event EventHandler<ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse>> ExecuteCommandCompleted;
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

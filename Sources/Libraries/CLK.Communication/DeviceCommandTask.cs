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
        private int _retryCount = int.MinValue;

        private int _expireMillisecond = int.MinValue;

        private DateTime _expireTime = DateTime.MaxValue;


        // Constructors
        internal DeviceCommandTask(int retryCount, int expireMillisecond) 
        {
            // Require
            if (retryCount <= 0) throw new InvalidOperationException();
            if (expireMillisecond <= 0) throw new InvalidOperationException();

            // Arguments           
            _retryCount = retryCount;
            _expireMillisecond = expireMillisecond;   
        }


        // Properties 
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
             

        // Methods
        public void ExecuteCommandAsync()
        {
            // Calculate
            lock (this)
            {
                // Require
                if (_retryCount <= 0) return;

                // RetryCount                
                _retryCount--;

                // ExpireTime
                _expireTime = DateTime.Now.AddMilliseconds(_expireMillisecond);
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

        protected abstract void BeginExecute();

        protected void EndExecute()
        {
            // Notify
            this.OnExecuteCommandEnded();
        }


        // Events
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
        internal DeviceCommandTask(Guid taskId, TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, TRequest request, int retryCount, int expireMillisecond)
            : base(retryCount, expireMillisecond)
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
        internal Guid TaskId { get; private set; }

        internal TDeviceAddress LocalDeviceAddress { get; private set; }

        internal TDeviceAddress RemoteDeviceAddress { get; private set; }

        internal TRequest Request { get; private set; }   


        // Methods
        protected override void BeginExecute()
        {
            // Create
            ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest> eventArgs = new ExecuteCommandArrivedEventArgs<TDeviceAddress, TRequest>(this.TaskId, this.LocalDeviceAddress, this.RemoteDeviceAddress, this.Request);

            // Notify
            this.OnExecuteCommandArrived(eventArgs);
        }

        internal void EndExecute(ExecuteCommandCompletedEventArgs<TDeviceAddress, TRequest, TResponse> eventArgs)
        {
            #region Contracts

            if (eventArgs == null) throw new ArgumentNullException();

            #endregion

            // Notify
            this.OnExecuteCommandCompleted(eventArgs);

            // Base
            this.EndExecute();            
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

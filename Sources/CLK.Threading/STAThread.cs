using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace CLK.Threading
{
    public class STAThread
    {
        // Enum
        private enum OperateState
        {
            Started,
            Stopping,
            Stopped,
        }


        // Fields
        private readonly object _syncRoot = new object();

        private OperateState _operateState = OperateState.Stopped;


        private Thread _executeThread = null;

        private OperateState _executeThreadState = OperateState.Stopped;

        private readonly ManualResetEvent _executeThreadEvent = new ManualResetEvent(true);

        private readonly BlockingQueue<Action> _executeActionQueue = new BlockingQueue<Action>();


        // Methods
        public void Start()
        {
            // OperateState
            lock (_syncRoot)
            {
                // Require
                if (_operateState != OperateState.Stopped) throw new InvalidOperationException();

                // State
                _operateState = OperateState.Started;
            }

            // ExecuteThread
            lock (_syncRoot)
            {
                _executeThreadEvent.Reset();
                _executeThreadState = OperateState.Started;
            }
            _executeThread = new Thread(this.ExecuteOperation);
            _executeThread.Name = string.Format("Class:{0}, SubThread:{1}, Id:{2}", "STAThread", "ExecuteThread", _executeThread.ManagedThreadId);
            _executeThread.IsBackground = false;
            _executeThread.Start();
        }

        public void Stop()
        {
            // OperateState         
            OperateState operateState = OperateState.Stopped;
            lock (_syncRoot)
            {
                // State
                operateState = _operateState;
                if (_operateState == OperateState.Started)
                {
                    _operateState = OperateState.Stopping;
                }
            }

            // ExecuteThread
            if (operateState == OperateState.Started)
            {               
                lock (_syncRoot)
                {
                    if (_executeThreadState != OperateState.Stopped)
                    {
                        _executeThreadState = OperateState.Stopping;
                        _executeActionQueue.Release();
                    }
                }
            }

            // Wait
            if (Thread.CurrentThread == _executeThread)
            {
                this.ExecuteOperation();
            }
            _executeThreadEvent.WaitOne();
            lock (_syncRoot) { _operateState = OperateState.Stopped; }
        }


        public void Post(SendOrPostCallback callback, object state)
        {
            #region Contracts

            if (callback == null) throw new ArgumentNullException();

            #endregion

            // Create
            Action action = delegate()
            {
                try
                {
                    callback(state);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Delegate:{0}, State:{1}, Message:{2}", callback.GetType(), "Exception", ex.Message));
                }
            };

            // Set
            lock (_syncRoot)
            {
                // Require
                if (_operateState != OperateState.Started) throw new InvalidOperationException();
                if (_executeThreadState != OperateState.Started) throw new InvalidOperationException();

                // Attach               
                _executeActionQueue.Enqueue(action);
            }
        }

        public void Send(SendOrPostCallback callback, object state)
        {
            #region Contracts

            if (callback == null) throw new ArgumentNullException();

            #endregion

            // Create 
            ManualResetEvent actionEvent = new ManualResetEvent(false);
            Action action = delegate()
            {
                try
                {
                    callback(state);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Delegate:{0}, State:{1}, Message:{2}", callback.GetType(), "Exception", ex.Message));
                }
                finally
                {
                    actionEvent.Set();
                }
            };

            // Set
            bool nowExecuteAction = true;
            lock (_syncRoot)
            {
                // Require
                if (_operateState != OperateState.Started) throw new InvalidOperationException();
                if (_executeThreadState != OperateState.Started) throw new InvalidOperationException();

                // Attach 
                if (Thread.CurrentThread != _executeThread)
                {
                    _executeActionQueue.Enqueue(action);
                    nowExecuteAction = false;
                }
            }

            // Execute
            if (nowExecuteAction == true)
            {
                action();
            }

            // Wait
            actionEvent.WaitOne();
        }

        
        private void ExecuteOperation()
        {
            // Operation
            try
            {
                // Execute
                while (true)
                {
                    // Detach
                    Action action = _executeActionQueue.Dequeue();

                    // Execute
                    if (action != null)
                    {
                        action();
                    }

                    // Exit
                    if (action == null)
                    {                        
                        lock (_syncRoot)
                        {
                            if (_executeThreadState != OperateState.Started)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            finally
            {
                // End
                lock (_syncRoot)
                {
                    _executeThreadState = OperateState.Stopped;
                }
                _executeThreadEvent.Set();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Threading
{
    public sealed class PortableStarterStoperLock
    {
        // Enum
        private enum OperateState
        {
            Started,
            Stopped,
            Operating,
            Failed,            
        }


        // Fields
        private readonly object _syncRoot = new object();

        private OperateState _operateState = OperateState.Stopped;


        // Properties                   
        public bool IsStarted
        {
            get
            {
                // State
                lock (_syncRoot)
                {
                    if (_operateState == OperateState.Started) return true;
                }

                // Return
                return false;
            }
        }


        // Methods       
        public bool EnterStartLock()
        {
            // State
            lock (_syncRoot)
            {
                if (_operateState != OperateState.Stopped) return false;
                _operateState = OperateState.Operating;
            }

            // Return
            return true;
        }

        public void ExitStartLock(bool isFailed = false)
        {
            // State
            lock (_syncRoot)
            {
                if (_operateState != OperateState.Operating) return;
                if (isFailed == true) _operateState = OperateState.Failed;
                if (isFailed == false) _operateState = OperateState.Started;
            }
        }


        public bool EnterStopLock()
        {
            // State
            lock (_syncRoot)
            {
                if (_operateState != OperateState.Started) return false;
                _operateState = OperateState.Operating;
            }

            // Return
            return true;
        }

        public void ExitStopLock(bool isFailed = false)
        {
            // State
            lock (_syncRoot)
            {
                if (_operateState != OperateState.Operating) return;
                if (isFailed == true) _operateState = OperateState.Failed;
                if (isFailed == false) _operateState = OperateState.Stopped;
            }
        }
    }
}

using CLK.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;

namespace CLK.Communication
{
    public sealed class Device<TDeviceAddress> : IDisposable
        where TDeviceAddress : DeviceAddress
    {
        // Fields        
        private readonly PortableStarterStoperLock _operateLock = new PortableStarterStoperLock();
                       
        private readonly PortableTimer _operateTimer = null;

        private readonly TDeviceAddress _localDeviceAddress = null;

        private readonly TDeviceAddress _remoteDeviceAddress = null;

        private readonly IEnumerable<DeviceCommand> _commandCollection = null;

        
        // Constructors
        internal Device(TDeviceAddress localDeviceAddress, TDeviceAddress remoteDeviceAddress, IDeviceCommandFactory<TDeviceAddress> commandFactory, PortableTimer operateTimer = null)
        {
            #region Contracts

            if (localDeviceAddress == null) throw new ArgumentNullException();
            if (remoteDeviceAddress == null) throw new ArgumentNullException();
            if (commandFactory == null) throw new ArgumentNullException();

            #endregion

            // Arguments   
            _localDeviceAddress = localDeviceAddress;
            _remoteDeviceAddress = remoteDeviceAddress;
            _operateTimer = operateTimer;

            // Create
            _commandCollection = commandFactory.CreateAll(_localDeviceAddress, _remoteDeviceAddress);
            if (_commandCollection == null) throw new InvalidOperationException();            
        }

        public void Dispose()
        {
            // Close
            this.Close();

            // Notify
            this.OnDeviceDisposed();                       
        }


        // Properties
        public TDeviceAddress LocalDeviceAddress { get { return _localDeviceAddress; } }

        public TDeviceAddress RemoteDeviceAddress { get { return _remoteDeviceAddress; } }


        // Methods
        internal void ApplyTimeTicked(long nowTicks)
        {
            // Command
            foreach (DeviceCommand command in _commandCollection)
            {
                command.ApplyTimeTicked(nowTicks);
            }
        }

        internal void Open()
        {
            // EnterStartLock
            if (_operateLock.EnterStartLock() == false) return;

            // Open
            try
            {
                // Command
                foreach (DeviceCommand command in _commandCollection)
                {
                    command.Start();
                }

                // Timer
                if (_operateTimer != null)
                {
                    _operateTimer.Ticked += this.Timer_Ticked;
                    _operateTimer.Start();
                }
            }
            finally
            {
                // ExitStartLock
                _operateLock.ExitStartLock();
            }
        }

        private void Close()
        {
            // EnterStopLock
            if (_operateLock.EnterStopLock() == false) return;

            // Close
            try
            {
                // Timer
                if (_operateTimer != null)
                {
                    _operateTimer.Stop();
                    _operateTimer.Ticked -= this.Timer_Ticked;
                }

                // Command
                foreach (DeviceCommand command in _commandCollection)
                {
                    command.Stop();
                }
            }
            finally
            {
                // ExitStopLock
                _operateLock.ExitStopLock();
            }
        }
        
        
        public TCommand GetCommand<TCommand>()
        {
            // Command
            return _commandCollection.OfType<TCommand>().FirstOrDefault();
        }
        

        // Handlers
        private void Timer_Ticked(object sender, EventArgs e)
        {
            // Apply
            this.ApplyTimeTicked(DateTime.Now.Ticks);
        }

        
        // Events
        internal event Action<Device<TDeviceAddress>> DeviceDisposed;
        private void OnDeviceDisposed()
        {
            var handler = this.DeviceDisposed;
            if (handler != null)
            {
                handler(this);
            }
        }
    }
}

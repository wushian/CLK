using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    internal sealed class ChannelServiceMediator
    {
        // Events
        public event EventHandler Connected;
        public void OnConnected(object sender)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();

            #endregion

            var handler = this.Connected;
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }

        public event EventHandler Disconnected;
        public void OnDisconnected(object sender)
        {
            #region Contracts

            if (sender == null) throw new ArgumentNullException();

            #endregion

            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }
    }
}
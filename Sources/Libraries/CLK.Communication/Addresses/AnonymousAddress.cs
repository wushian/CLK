using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Communication
{
    public sealed class AnonymousAddress : DeviceAddress
    {
        // Singleton
        private static AnonymousAddress _instance = null;

        public static AnonymousAddress Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AnonymousAddress();
                }
                return _instance;
            }
        }


        // Methods
        public override bool EqualAddress(DeviceAddress address)
        {
            #region Contracts

            if (address == null) throw new ArgumentNullException();

            #endregion

            // Return
            return true;
        }
    }
}

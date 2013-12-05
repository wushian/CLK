using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.ServiceModel
{
    public static class ExceptionTransformExtension
    {
        // Convert  
        public static Exception Convert(this Exception source)
        {
            #region Contracts

            if (source == null) throw new ArgumentNullException();

            #endregion

            // Create
            var result = new Exception(source.Message);

            // Return
            return result;
        }
    }
}

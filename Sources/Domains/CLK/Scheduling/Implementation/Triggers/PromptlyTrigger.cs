using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Scheduling
{
    public sealed class PromptlyTrigger: ITaskTrigger
    {
        // Methods
        public bool Verify(DateTime executeTime, DateTime lastExecuteTime)
        {
            // Return
            return true;
        }
    }
}

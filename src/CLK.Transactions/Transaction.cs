﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Transactions
{
    public interface Transaction : IDisposable
    {
        // Methods
        void Complete();
    }
}

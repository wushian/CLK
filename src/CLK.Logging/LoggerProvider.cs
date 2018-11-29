﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Logging
{
    public interface LoggerProvider : IDisposable
    {
        // Constructors
        void Start();


        // Methods
        Logger<TCategory> Create<TCategory>();
    }
}

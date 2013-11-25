using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.ComponentModel
{
    public delegate void TypedEventHandler<in TSender>(TSender sender, EventArgs e);

    public delegate void TypedEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e) where TEventArgs : EventArgs;
}

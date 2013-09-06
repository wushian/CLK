using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CLK.ComponentModel
{
    public delegate void TypedPropertyChangedEventHandler<in TSender>(TSender sender, PropertyChangedEventArgs e);
}

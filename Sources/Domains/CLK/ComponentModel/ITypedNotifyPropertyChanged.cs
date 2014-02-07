using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.ComponentModel
{
    public interface ITypedNotifyPropertyChanged<TSender>
    {
        event TypedPropertyChangedEventHandler<TSender> TypedPropertyChanged;
    }
}

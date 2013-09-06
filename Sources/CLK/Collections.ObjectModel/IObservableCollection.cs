using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CLK.Collections.ObjectModel
{
    public interface IObservableCollection<T> : IEnumerable<T>, ICollection<T>, IList<T>, System.Collections.Specialized.INotifyCollectionChanged, System.ComponentModel.INotifyPropertyChanged
    {

    }
}

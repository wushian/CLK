using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Scheduling
{
    public interface ITaskRecordRepository<T>
    {
        // Methods
        void Add(TaskRecord<T> taskRecord);
    }
}

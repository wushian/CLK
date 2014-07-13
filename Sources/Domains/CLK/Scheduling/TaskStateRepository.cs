using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Scheduling
{
    public interface ITaskStateRepository<T>
    {
        // Methods
        void Set(TaskState<T> taskState);

        TaskState<T> Get(T taskSettingId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Scheduling
{
    public sealed class TaskState<T>
    {
        // Constructors
        public TaskState(T taskSettingId)
        {
            // Arguments
            this.TaskSettingId = taskSettingId;
            this.LastExecuteTime = DateTime.MinValue;
        }

        public TaskState(T taskSettingId, DateTime lastExecuteTime)
        {
            // Arguments
            this.TaskSettingId = taskSettingId;
            this.LastExecuteTime = lastExecuteTime;
        }


        // Properties
        public T TaskSettingId { get; private set; }

        public DateTime LastExecuteTime { get; internal set; }
    }
}

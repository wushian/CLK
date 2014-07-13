using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Scheduling
{
    public sealed class TaskRecord<T>
    {
        // Constructors
        public TaskRecord(T taskSettingId, DateTime executeTime) 
        {
            // Arguments
            this.TaskSettingId = taskSettingId;
            this.ExecuteTime = executeTime;
        }

        public TaskRecord(T taskSettingId, DateTime executeTime, Exception executeError)
        {
            #region Contracts

            if (executeError == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskSettingId = taskSettingId;
            this.ExecuteTime = executeTime;
            this.ExecuteError = executeError;
        }


        // Properties
        public T TaskSettingId { get; private set; }

        public DateTime ExecuteTime { get; private set; }

        public Exception ExecuteError { get; private set; }
    }
}

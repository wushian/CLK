using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Scheduling
{
    public sealed class TaskRecord
    {
        // Constructors
        public TaskRecord(string taskSettingId, DateTime executeTime) 
        {
            #region Contracts

            if (string.IsNullOrEmpty(taskSettingId) == true) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskSettingId = taskSettingId;
            this.ExecuteTime = executeTime;
        }

        public TaskRecord(string taskSettingId, DateTime executeTime, Exception error)
        {
            #region Contracts

            if (string.IsNullOrEmpty(taskSettingId) == true) throw new ArgumentNullException();
            if (error == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskSettingId = taskSettingId;
            this.ExecuteTime = executeTime;
            this.Error = error;
        }


        // Properties
        public string TaskSettingId { get; private set; }

        public DateTime ExecuteTime { get; private set; }

        public Exception Error { get; private set; }
    }
}

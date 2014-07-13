using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Scheduling
{
    public class TaskSetting<T>
    {
        // Constructors
        public TaskSetting(T taskSettingId, string taskSettingName, ITaskTrigger taskTrigger, ITaskAction taskAction)
        {
            #region Contracts

            if (string.IsNullOrEmpty(taskSettingName) == true) throw new ArgumentNullException();
            if (taskTrigger == null) throw new ArgumentNullException();
            if (taskAction == null) throw new ArgumentNullException();

            #endregion

            // Arguments
            this.TaskSettingId = taskSettingId;
            this.TaskSettingName = taskSettingName;
            this.TaskTrigger = taskTrigger;
            this.TaskAction = taskAction;
        }


        // Properties
        public T TaskSettingId { get; private set; }

        public string TaskSettingName { get; private set; }

        public ITaskTrigger TaskTrigger { get; private set; }

        public ITaskAction TaskAction { get; private set; }
    }
}

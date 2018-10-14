using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Activities
{
    public abstract class ActivityFactory<TActivity, TActivityModel> : ViewFactory<TActivity, TActivityModel>
        where TActivity : Activity
        where TActivityModel : ActivityModel
    {
        // Constructors
        public ActivityFactory(Func<TActivity> createAction) : base(createAction)
        {
            
        }
    }
}

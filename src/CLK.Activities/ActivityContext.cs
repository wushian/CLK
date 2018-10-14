using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Activities
{
    public partial class ActivityContext : IDisposable
    {
        // Singleton 
        private static ActivityContext _current;

        internal static ActivityContext Current
        {
            get
            {
                // Require
                if (_current == null) throw new InvalidOperationException("_current=null");

                // Return
                return _current;
            }
        }

        public static ActivityContext Initialize(ActivityContext activityContext)
        {
            #region Contracts

            if (activityContext == null) throw new ArgumentException();

            #endregion

            // Default
            _current = activityContext;

            // Return
            return _current;
        }
    }

    public partial class ActivityContext : IDisposable
    {
        // Fields
        private readonly Dictionary<Type, ViewFactory> _viewFactoryDictionary = new Dictionary<Type, ViewFactory>();


        // Constructors
        public ActivityContext(IEnumerable<ViewFactory> viewFactoryList)
        {
            #region Contracts

            if (viewFactoryList == null) throw new ArgumentException();

            #endregion

            // ViewFactoryDictionary
            foreach (var viewFactory in viewFactoryList)
            {
                // Add
                _viewFactoryDictionary.Add(viewFactory.ModelType, viewFactory);
            }
        }

        public void Start()
        {

        }

        public void Dispose()
        {
           
        }


        // Properties
        public Dictionary<Type, ViewFactory> ViewFactoryDictionary { get; private set; }
    }
}

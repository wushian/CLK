using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CLK.Activities
{
    public class ViewConverter : IValueConverter
    {
        // Fields
        private readonly Dictionary<Type, ViewFactory> _viewFactoryDictionary = null;


        // Constructors
        public ViewConverter()
        {
            // Attach
            _viewFactoryDictionary = ActivityContext.Current.ViewFactoryDictionary;
        }


        // Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert
            var convertResult = this.ConvertModel(value);
            if (convertResult == null) throw new InvalidOperationException("convertResult=null");

            // View
            var view = convertResult.Item1;
            if (view == null) throw new InvalidOperationException("view=null");

            // ViewModel
            var viewModel = convertResult.Item2;
            if (viewModel == null) throw new InvalidOperationException("viewModel=null");

            // Initailize
            view.DataContext = viewModel;

            // Return
            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Throw
            throw new NotImplementedException();
        }


        private Tuple<View, object> ConvertModel(object viewModel = null)
        {
            // NotFoundView
            if (viewModel == null)
            {
                return new Tuple<View, object>
                (
                    new NotFoundView(),
                    new NotFoundViewModel("null")
                );
            }

            // ViewFactory
            if (_viewFactoryDictionary.ContainsKey(viewModel.GetType()) == false)
            {
                return new Tuple<View, object>
                (
                    new NotFoundView(),
                    new NotFoundViewModel(viewModel.GetType().FullName)
                );
            }
            var viewFactory = _viewFactoryDictionary[viewModel.GetType()];

            // View
            var view = viewFactory.Create();
            if (view == null)
            {
                return new Tuple<View, object>
                (
                    new NotFoundView(),
                    new NotFoundViewModel(viewModel.GetType().FullName)
                );
            }

            // Return
            return new Tuple<View, object>
            (
                view,
                viewModel
            );
        }
    }
}

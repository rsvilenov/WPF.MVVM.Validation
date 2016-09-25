using System;
using Validation.Contracts;

namespace Validation.ViewModel
{
    public partial class BaseViewModel : INotifyDataErrorOrder
    {
        EventHandler<DataErrorOrderEventArgs> _dataErrorOrderChanged;

        event EventHandler<DataErrorOrderEventArgs> INotifyDataErrorOrder.DataErrorOrderChanged
        {
            add
            {
                _dataErrorOrderChanged += value;
            }

            remove
            {
                _dataErrorOrderChanged -= value;
            }
        }

        void NotifyDataErrorOrderChanged(string propertyName, int order)
        {
            if (_dataErrorOrderChanged != null)
                _dataErrorOrderChanged(this, new DataErrorOrderEventArgs(propertyName, order));
        }
    }
}

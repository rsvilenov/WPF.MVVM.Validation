using System;
using Validation.Contracts;

namespace Validation.ViewModel
{
    public partial class BaseViewModel : INotifyDataValidationStarted
    {

        EventHandler<DataValidationStartedEventArgs> _dataValidationStarted;
        event EventHandler<DataValidationStartedEventArgs> INotifyDataValidationStarted.DataValidationStarted
        {
            add
            {
                _dataValidationStarted += value;
            }

            remove
            {
                _dataValidationStarted -= value;
            }
        }

        void NotifyDataValidationStarted(string propertyName, bool fieldValidated)
        {
            if (_dataValidationStarted != null)
                _dataValidationStarted(this, new DataValidationStartedEventArgs(propertyName, fieldValidated));
        }
    }
}

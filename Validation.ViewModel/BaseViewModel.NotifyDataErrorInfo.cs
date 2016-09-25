using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace Validation.ViewModel
{
    public partial class BaseViewModel : INotifyDataErrorInfo
    {

        event EventHandler<DataErrorsChangedEventArgs> _errorsChanged;
        /// <summary>
        /// Do not easily allow users (subclasses of our base class) to trigger this event on their own
        /// </summary>
        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add
            {
                _errorsChanged += value;
            }
            remove
            {
                _errorsChanged -= value;
            }
        }

        /// <summary>
        /// This is the allowed way of triggering ErrorsChanged
        /// </summary>
        /// <param name="propName"></param>
        void RaiseErrorsChanged(string propName)
        {
            if (_errorsChanged != null)
                _errorsChanged(this, new DataErrorsChangedEventArgs(propName));
        }

        /// <summary>
        /// Indicates whether there are any validation errors
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return ValidationMessages.Any(x => x.Type == ValidationMessageType.Error);
            }
        }

        /// <summary>
        /// Gets all validation errors for a property
        /// </summary>
        /// <param name="propertyName">the name of the property</param>
        /// <returns>an IEnumerable of all validation errors for 'propertyName'</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return ValidationMessages.
                   Where(x => x.PropertyName == propertyName && x.Type == ValidationMessageType.Error).
                   Select(x => x.Message).
                   ToList();
        }

    }
}

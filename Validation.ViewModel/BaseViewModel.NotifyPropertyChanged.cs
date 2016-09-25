using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Validation.ViewModel
{
    public partial class BaseViewModel : INotifyPropertyChanged
    {
        event PropertyChangedEventHandler _propertyChanged;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propName = "")
        {
            if (this.GetType().GetProperty(propName) == null)
                throw new InvalidOperationException("This method can be called only from within a property.");

            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public void RaisePropertyChanged(Expression<Func<object>> exp)
        {
            string propName = Utils.GetPropertyNameFromLambda(exp);
            if (propName == null)
                throw new ArgumentException("Usage: AddValidationMessage(() => Property");

            RaisePropertyChanged(propName);
        }
    }
}

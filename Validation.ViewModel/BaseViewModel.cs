using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace Validation.ViewModel
{
    public partial class BaseViewModel
    {
        ObservableCollection<ValidationMessage> _validationMessages = new ObservableCollection<ValidationMessage>();
        public ObservableCollection<ValidationMessage> ValidationMessages
        {
            get { return _validationMessages; }
            private set
            {
                _validationMessages = value;
                RaisePropertyChanged();
            }
        }
        
        public bool IsValidationSuspended
        {
            get;
            private set;
        }

        protected void SuspendValidation()
        {
            IsValidationSuspended = true;
        }

        protected void ResumeValidation()
        {
            IsValidationSuspended = false;
        }

        public void ClearValidationMessages(Expression<Func<object>> exp, ValidationMessageType type = ValidationMessageType.Error)
        {
            string propName = Utils.GetPropertyNameFromLambda(exp);
            if (propName == null)
                throw new ArgumentException("Usage: ClearValidationMessages(() => Property [, type]");
            ClearValidationMsgs(propName, type);
        }

        public void ClearValidationMessages([CallerMemberName] string propName = "", ValidationMessageType type = ValidationMessageType.Error)
        {
            if (this.GetType().GetProperty(propName) == null)
               throw new InvalidOperationException("This method can be called only from within a property.");

            ClearValidationMsgs(propName, type);
        }

        void ClearValidationMsgs(string propName, ValidationMessageType type)
        {
            var msgs = ValidationMessages.Where(x => x.PropertyName == propName && x.Type == type).ToList();
            if (msgs.Count > 0)
            {
                foreach (var msg in msgs)
                    ValidationMessages.Remove(msg);

                RaiseErrorsChanged(propName);
            }
        }

        public void AddValidationMessage(string message, [CallerMemberName] string propName = "", ValidationMessageType type = ValidationMessageType.Error)
        {
            if (this.GetType().GetProperty(propName) == null)
                throw new InvalidOperationException("This method can be called only from within a property.");

            AddValidationMsg(message, propName, type);
        }
        

        public void AddValidationMessage(Expression<Func<object>> exp, string message, ValidationMessageType type = ValidationMessageType.Error)
        {
            string propName = Utils.GetPropertyNameFromLambda(exp);
            if (propName == null)
                throw new ArgumentException("Usage: AddValidationMessage(() => Property, message [, type]");

            AddValidationMsg(message, propName, type);
        }

        void AddValidationMsg(string msg, string propName, ValidationMessageType type)
        {
            if (IsValidationSuspended)
                return;

            ValidationMessages.Add(new ValidationMessage(msg, propName, type));
            
            RaiseErrorsChanged(propName);
        }

        public bool Validate()
        {
            var propertiesToValidate = PropertyProcessedStates.Where(x => !x.Value).Select(x => x.Key).ToList();
            foreach (string propertyName in propertiesToValidate)
            {
                ClearValidationMessages(propertyName);
                NotifyDataValidationStarted(propertyName, false);
                RaiseErrorsChanged(propertyName);
            }

            var rest = PropertyProcessedStates.
                Where(x => !propertiesToValidate.Contains(x.Key)).
                Select(x => x.Key).
                ToList();
            foreach (var propertyName in rest)
            {
                NotifyDataValidationStarted(propertyName, true);
            }

            int order = 0;
            var errorMessages = ValidationMessages.
                Where(x => x.Type == ValidationMessageType.Error).
                ToDictionary(x => order++, y => y.PropertyName);

            foreach (var key in errorMessages)
            {
                NotifyDataErrorOrderChanged(key.Value, key.Key);
            }

            return order == 0;
        }

    }
}

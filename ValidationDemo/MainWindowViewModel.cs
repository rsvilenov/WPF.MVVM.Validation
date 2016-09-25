using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Validation.ViewModel;

namespace ValidationDemo
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
        {
            ValidationMessages.CollectionChanged += (a, e) => RaisePropertyChanged(() => ValidationMessage);
        }

        string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string ValidationMessage
        {
            get
            {
                if (ValidationMessages.Count > 0)
                    return ValidationMessages[0].Message;
                return "";
            }
        }

        string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                ClearValidationMessages();
                if (string.IsNullOrEmpty(_email))
                {
                    AddValidationMessage("The email is mandatory.");
                }
                else
                    if (!_email.Contains('@') || !_email.Contains('.'))
                {
                    AddValidationMessage("The email is incorrect.");
                }
            }
        }

        string _address;
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

    }
}

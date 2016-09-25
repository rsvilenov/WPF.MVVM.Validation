using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Contracts
{
    public class DataValidationStartedEventArgs : PropertyChangedEventArgs
    {
        public DataValidationStartedEventArgs(string propertyName, bool fieldValidated) :
            base (propertyName)
        {
            FieldValidated = fieldValidated;
        }

        public bool FieldValidated { get; set; }
    }
}

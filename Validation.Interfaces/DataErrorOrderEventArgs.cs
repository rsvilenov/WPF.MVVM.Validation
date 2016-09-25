using System;
using System.ComponentModel;

namespace Validation.Contracts
{
    public class DataErrorOrderEventArgs : PropertyChangedEventArgs
    {
        public DataErrorOrderEventArgs(string propertyName, int errorOrder) : 
            base (propertyName)
        {
            ErrorOrder = errorOrder;
        }
        
        public int ErrorOrder { get; set; }
    }
}


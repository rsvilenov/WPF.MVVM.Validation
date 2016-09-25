using System;
using System.Windows;

namespace Validation.MarkupExtention
{
    public class DataContextChangedEventArgs : EventArgs
    {
        public object NewDataContext { get; set; }
        public object OldDataContext { get; set; }

        public DataContextChangedEventArgs(object oldValue, object newValue)
        {
            OldDataContext = oldValue;
            NewDataContext = newValue;
        }
    }
}
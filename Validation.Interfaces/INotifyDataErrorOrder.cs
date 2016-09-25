using System;

namespace Validation.Contracts
{
    public interface INotifyDataErrorOrder
    {
        event EventHandler<DataErrorOrderEventArgs> DataErrorOrderChanged;
    }
}

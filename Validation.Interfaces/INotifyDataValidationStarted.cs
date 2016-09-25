using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Contracts
{
    public interface INotifyDataValidationStarted
    {
        event EventHandler<DataValidationStartedEventArgs> DataValidationStarted;
    }
}

using System.Collections.Generic;
using Validation.Contracts;

namespace Validation.ViewModel
{
    public partial class BaseViewModel : IProcessedStateProvider
    {

        Dictionary<string, bool> PropertyProcessedStates { get; set; } = new Dictionary<string, bool>();


        void IProcessedStateProvider.AddMonitoredField(string propertyName)
        {
            if (!PropertyProcessedStates.ContainsKey(propertyName))
                PropertyProcessedStates.Add(propertyName, false);
        }

        void IProcessedStateProvider.SetMonitoredFieldVisited(string propertyName)
        {
            PropertyProcessedStates[propertyName] = true;
        }
    }
}

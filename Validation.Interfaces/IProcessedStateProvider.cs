namespace Validation.Contracts
{
    public interface IProcessedStateProvider
    {
        void AddMonitoredField(string propertyName);
        void SetMonitoredFieldVisited(string propertyName);
    }
}

namespace Validation.ViewModel
{
    public class ValidationMessage
    {
        public ValidationMessage(string msg, string propName,
            ValidationMessageType type = ValidationMessageType.Error)
        {
            Message = msg;
            Type = type;
            PropertyName = propName;
        }

        public string PropertyName { get; set; }
        public string Message { get; set; }
        public ValidationMessageType Type { get; set; }
    }

}

using Newtonsoft.Json;

namespace Smarthome.WS.interfaces;
using System.ComponentModel.DataAnnotations;

public class SendMessageDto<TPayload>
{
    public string Type { get; set; }
    [RequiredIf("Type", "Info", ErrorMessage = "Case is required for Type 'Info'")]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Case { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }
    public TPayload  Payload { get; set; }
}
public static class CaseEnum
{
    public const string Connected = "Connected";
    public const string Disconnected = "Disconnected";
    public const string Info = "Info";
    public const string Error = "Error";
}
public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string _propertyName;
    private readonly object _desiredValue;

    public RequiredIfAttribute(string propertyName, object desiredValue)
    {
        _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        _desiredValue = desiredValue;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_propertyName);

        if (property == null)
        {
            return new ValidationResult($"Unknown property: {_propertyName}");
        }

        var propertyValue = property.GetValue(validationContext.ObjectInstance, null);

        if (Equals(propertyValue, _desiredValue) && value == null)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
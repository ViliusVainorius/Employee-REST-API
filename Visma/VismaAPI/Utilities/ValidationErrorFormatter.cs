namespace VismaAPI.Utilities;

public static class ValidationErrorFormatter
{
    public static List<string> FormatValidationErrors(IEnumerable<FluentValidation.Results.ValidationFailure> errors)
    {
        return errors
            .Select(x => $"{x.PropertyName}: {x.ErrorMessage} Severity: {x.Severity}")
            .ToList();
    }
}

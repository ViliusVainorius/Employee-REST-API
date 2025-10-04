namespace VismaAPI.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; } = [];

    public bool HasErrors => Errors.Count > 0;

    public ValidationException(List<string> errors)
    {
        Errors = errors;
    }
}

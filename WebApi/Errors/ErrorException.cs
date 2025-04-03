namespace WebApi.Errors;

public class ErrorException : Exception, IError
{
    public ErrorException(IError error) : base(error.Message)
    {
        ErrorMessage = error.Message;
        StatusCode = error.StatusCode;
        Code = error.Code;
    }
    public int? StatusCode { get; }
    public string Code { get; }
    public string ErrorMessage { get; }
}
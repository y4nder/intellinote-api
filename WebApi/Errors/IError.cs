namespace WebApi.Errors;

public interface IError : IStatusCodeHttpResult 
{
    public string Code { get; }
    public string Message { get; }
}
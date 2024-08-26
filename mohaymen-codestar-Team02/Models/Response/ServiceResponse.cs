namespace mohaymen_codestar_Team02.Models;

public class ServiceResponse<T>
{
    public ServiceResponse(T? data, ApiResponseType type, string message)
    {
        Data = data;
        Type = type;
        Message = message;
    }

    public T? Data { get; set; }
    public ApiResponseType Type { get; set; }
    public string Message { get; set; }
}
namespace mohaymen_codestar_Team02.Models;

public class ServiceResponse<T>
{
    public T Data { get; set; }
    public ApiResponse Type { get; set; }
    public string Message { get; set; } = string.Empty;
}
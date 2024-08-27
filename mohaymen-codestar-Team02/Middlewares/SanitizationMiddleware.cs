using System.Text;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;

namespace mohaymen_codestar_Team02.Middlewares;

public class SanitizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HtmlSanitizer _sanitizer;

    public SanitizationMiddleware(RequestDelegate next)
    {
        _next = next;
        _sanitizer = new HtmlSanitizer();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
        {
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                var type = GetRequestDtoType(context);
                if (type != null)
                {
                    var sanitizedBody = SanitizeRequestBody(body, type);
                    var buffer = Encoding.UTF8.GetBytes(sanitizedBody);

                    context.Request.Body = new MemoryStream(buffer);
                    context.Request.Body.Position = 0; 
                }
            }
        }

        await _next(context);
    }

    private Type? GetRequestDtoType(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (actionDescriptor != null)
        {
            var parameters = actionDescriptor.Parameters;
            var dtoParameter = parameters.FirstOrDefault(p => p.ParameterType.IsClass && p.ParameterType != typeof(string));
            return dtoParameter?.ParameterType;
        }
        return null;
    }

    private string SanitizeRequestBody(string body, Type type)
    {
        object sanitizedDto;
        if (type == typeof(List<string>))
        {
            var dto = JsonConvert.DeserializeObject<IEnumerable<string>>(body);
            sanitizedDto = SanitizeEnumerable(dto);
        }
        else
        {
            var dto = JsonConvert.DeserializeObject(body, type);
            sanitizedDto = SanitizeDto(dto);
        }

        return JsonConvert.SerializeObject(sanitizedDto);
    }

    private IEnumerable<string> SanitizeEnumerable(IEnumerable<string> dto)
    {
        return dto.Select(str => _sanitizer.Sanitize(str));
    }

    private object SanitizeDto(object dto)
    {
        var properties = dto.GetType().GetProperties().Where(p => p.PropertyType == typeof(string) && p.CanWrite && p.CanRead);

        foreach (var property in properties)
        {
            var value = (string)property.GetValue(dto);
            if (value != null)
            {
                property.SetValue(dto, _sanitizer.Sanitize(value));
            }
        }

        return dto; 
    }
}


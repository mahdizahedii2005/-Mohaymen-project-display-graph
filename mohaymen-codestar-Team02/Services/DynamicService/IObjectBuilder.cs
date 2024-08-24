namespace mohaymen_codestar_Team02.Services.DynamicService;

public interface IObjectBuilder
{
    public object CreateDynamicObject(Type objectType, Dictionary<string, string> attributeValues);
}
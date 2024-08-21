using mohaymen_codestar_Team02.Data;

namespace mohaymen_codestar_Team02.Services.TokenService;

public class ObjectBuilder
{

    public object CreateDynamicObject(Type objectType, Dictionary<string, string> attributeValues)
    {
        var dynamicObject = Activator.CreateInstance(objectType);

        foreach (var attributeValue in attributeValues)
        {
            objectType.GetProperty(attributeValue.Key).SetValue(dynamicObject, attributeValue.Value);
        }

        return dynamicObject;
    }

    public object CreateDynamicObject1(Type objectType, Dictionary<string, string> attributeValues, Type vertexType, object source, object destination)
    {
        var dynamicObject = Activator.CreateInstance(objectType);

        vertexType.GetProperty("Source").SetValue(vertexType, source);
        vertexType.GetProperty("Target").SetValue(vertexType, destination);

        foreach (var attributeValue in attributeValues)
        {
            objectType.GetProperty(attributeValue.Key).SetValue(dynamicObject, attributeValue.Value);
        }

        return dynamicObject;
    }
}
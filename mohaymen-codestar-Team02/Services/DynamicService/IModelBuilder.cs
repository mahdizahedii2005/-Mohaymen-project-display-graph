using System.Reflection.Emit;

namespace mohaymen_codestar_Team02.Services.DynamicService;

public interface IModelBuilder
{
    void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType);
    TypeBuilder GetTypeBuilder(string className);
    public Type CreateDynamicClass(string className, Dictionary<string, Type> fieldNamesTypes, Type interfaceType);
}
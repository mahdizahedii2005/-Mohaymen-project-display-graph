using System.Reflection.Emit;

namespace mohaymen_codestar_Team02.Services.DynamicService;

public interface IModelBuilder
{
    public Type CreateDynamicClass(string className, Dictionary<string, Type> fieldNamesTypes, Type interfaceType);
}
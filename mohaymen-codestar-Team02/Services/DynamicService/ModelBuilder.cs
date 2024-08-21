using System.Reflection;
using System.Reflection.Emit;
using mohaymen_codestar_Team02.Services.DynamicService;

namespace mohaymen_codestar_Team02.Services.DynamicService;

public class ModelBuilder : IModelBuilder
{
    public Type CreateDynamicClass(string className, Dictionary<string, Type> fieldNamesTypes, Type interfaceType)
    {
        var typeBuilder = GetTypeBuilder(className);

        if (interfaceType != null) typeBuilder.AddInterfaceImplementation(interfaceType);

        foreach (var fieldNamesType in fieldNamesTypes)
            CreateProperty(typeBuilder, fieldNamesType.Key, fieldNamesType.Value);

        return typeBuilder.CreateType();
    }

    private TypeBuilder GetTypeBuilder(string className)
    {
        var typeSignature = className;
        var assemblyName = new AssemblyName(typeSignature);
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        return moduleBuilder.DefineType(typeSignature, TypeAttributes.Public | TypeAttributes.Class);
    }

    private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
    {
        var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

        var propertyBuilder =
            typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
        var getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
            Type.EmptyTypes);
        var getIl = getPropMthdBldr.GetILGenerator();

        getIl.Emit(OpCodes.Ldarg_0);
        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
        getIl.Emit(OpCodes.Ret);

        var setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null,
            new[] { propertyType });
        var setIl = setPropMthdBldr.GetILGenerator();

        setIl.Emit(OpCodes.Ldarg_0);
        setIl.Emit(OpCodes.Ldarg_1);
        setIl.Emit(OpCodes.Stfld, fieldBuilder);
        setIl.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getPropMthdBldr);
        propertyBuilder.SetSetMethod(setPropMthdBldr);
    }
}
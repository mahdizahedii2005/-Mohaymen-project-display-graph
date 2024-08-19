using System.Dynamic;

namespace mohaymen_codestar_Team02.Services.StoreData.Abstraction;

public interface IStoreDataService
{
    bool StoreEntity(Type? type);
    bool StoreAttribute(IEnumerable<string>? attributes);
    bool StoreValues(IEnumerable<ExpandoObject>? records);
}
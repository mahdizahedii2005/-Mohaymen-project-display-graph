using System.Dynamic;

namespace mohaymen_codestar_Team02.Services;

public interface IStoreDataService
{
    void StoreEntity(Type type);
    void StoreAttribute(IEnumerable<string> attributes);
    void StoreValues(IEnumerable<ExpandoObject> records);
}
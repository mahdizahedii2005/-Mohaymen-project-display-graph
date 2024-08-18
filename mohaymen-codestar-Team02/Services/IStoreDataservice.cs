namespace WebApplication13.Services;

public interface IStoreDataService
{
    void StoreEntity(Type type);
    void StoreAttribute(IEnumerable<string> attribuites);
    void StoreValues<T>(IEnumerable<T> records);
}
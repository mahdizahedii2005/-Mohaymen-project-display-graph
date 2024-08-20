using System.Dynamic;

namespace mohaymen_codestar_Team02.Services.StoreData.Abstraction;

public interface IStoreDataService
{
    Task<bool> StoreFileData(string entityName, string dataFile, long dataGroupId);
}
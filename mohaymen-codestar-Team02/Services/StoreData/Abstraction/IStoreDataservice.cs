using System.Dynamic;

namespace mohaymen_codestar_Team02.Services.StoreData.Abstraction;

public interface IStoreDataService
{
    bool StoreFileData(string dataFile, long dataGroupId);
}
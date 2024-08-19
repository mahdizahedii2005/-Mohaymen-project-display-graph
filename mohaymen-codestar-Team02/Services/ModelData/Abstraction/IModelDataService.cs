using System.Dynamic;

namespace mohaymen_codestar_Team02.Services.ModelData.Abstraction;

public interface IModelDataService
{
    Type GetType(string fileData);
    IEnumerable<string> GetFields(string fieldData);
    IEnumerable<ExpandoObject> GetRecords(string recordData);
}
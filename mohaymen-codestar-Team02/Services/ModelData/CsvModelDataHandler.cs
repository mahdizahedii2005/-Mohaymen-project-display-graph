using System.Dynamic;
using mohaymen_codestar_Team02.Services.ModelData.Abstraction;

namespace mohaymen_codestar_Team02.Services.ModelData;

public class CsvModelDataHandler : IModelDataService
{
    public Type GetType(string EntityData)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetFields(string fieldData)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ExpandoObject> GetRecords(string recordData)
    {
        throw new NotImplementedException();
    }
}
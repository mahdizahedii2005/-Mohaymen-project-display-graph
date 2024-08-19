using System.Dynamic;

namespace mohaymen_codestar_Team02.Services.ModelData.Abstraction;

public interface IModelDataService
{
    IEnumerable<ExpandoObject> GetRecords(string recordData);
}
using System.Dynamic;

namespace WebApplication13.Services;

public interface IModelDataService
{
    Type GetType(string EntityData);
    IEnumerable<string> GetFields(string fieldData);
    IEnumerable<ExpandoObject> GetRecords(string recordData);
}
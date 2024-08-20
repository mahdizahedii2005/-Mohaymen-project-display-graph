using System.Dynamic;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class EdgeStorerCsv : IEdageStorer
{
    private readonly IServiceProvider? _serviceProvider;
    public EdgeStorerCsv(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task<bool> StoreFileData(string entityName, string dataFile, long dataGroupId)
    {
        using var scope = _serviceProvider.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        try
        {
            var edgeEntity = new EdgeEntity(entityName, dataGroupId);
            List<EdgeAttribute> edgeAttributes = new();
            List<EdgeValue> edgeValues = new();
            await dataContext.EdgeEntities.AddAsync(edgeEntity);
            await dataContext.SaveChangesAsync();
            using (var reader = new StringReader(dataFile))
            {
                var headerLine = reader.ReadLine();
                if (headerLine == null) return false;

                var headers = headerLine.Split(',');
                foreach (var att in headers) edgeAttributes.Add(new EdgeAttribute(att, edgeEntity.Id));

                foreach (var attribute in edgeAttributes) await dataContext.EdgeAttributes.AddAsync(attribute);

                await dataContext.SaveChangesAsync();
                string? line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var objectId = Guid.NewGuid().ToString();
                    var values = line.Split("\",\"");
                    values[0] = values[0].Substring(1);
                    var lastWord = values[values.Length - 1];
                    values[values.Length - 1] = lastWord.Substring(0, lastWord.Length - 1);
                    for (var i = 0; i < values.Length; i++)
                        edgeValues.Add(new EdgeValue(values[i], edgeAttributes[i].Id, objectId));
                }
            }

            foreach (var value in edgeValues) await dataContext.EdgeValues.AddAsync(value);

            await dataContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
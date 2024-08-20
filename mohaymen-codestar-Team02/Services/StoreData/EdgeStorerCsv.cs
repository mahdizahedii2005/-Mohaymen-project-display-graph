using System.Dynamic;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class EdgeStorerCsv(IServiceProvider? serviceProvider) : IEdageStorer
{
    public async Task<bool> StoreFileData(string entityName, string dataFile, long dataGroupId)
    {
        using var scope = serviceProvider.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        try
        {
            EdgeEntity edgeEntity = new EdgeEntity(entityName, dataGroupId);
            List<EdgeAttribute> edgeAttributes = new List<EdgeAttribute>();
            List<EdgeValue> edgeValues = new List<EdgeValue>();
            using (var reader = new StringReader(dataFile))
            {
                string? headerLine = reader.ReadLine();
                if (headerLine == null)
                {
                    return false;
                }

                var headers = headerLine.Split(',');
                foreach (var att in headers)
                {
                    edgeAttributes.Add(new EdgeAttribute(att, edgeEntity.Id));
                }

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var objectId = Guid.NewGuid().ToString();
                    var values = line.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        edgeValues.Add(new EdgeValue(values[i], edgeAttributes[i].Id, objectId));
                    }
                }
            }

            await dataContext.EdgeEntities.AddAsync(edgeEntity);
            foreach (var attribute in edgeAttributes)
            {
                await dataContext.EdgeAttributes.AddAsync(attribute);
            }

            foreach (var value in edgeValues)
            {
                await dataContext.EdgeValues.AddAsync(value);
            }

            await dataContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
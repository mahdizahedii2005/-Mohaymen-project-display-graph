using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class VertexStorerCsv : IVertexStorer
{
    private readonly IServiceProvider _serviceProvider;

    public VertexStorerCsv(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> StoreFileData(string entityName, string dataFile, long dataGroupId)
    {
        using var scope = _serviceProvider.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        try
        {
            var edgeEntity = new VertexEntity(entityName, dataGroupId);
            List<VertexAttribute> edgeAttributes = new();
            List<VertexValue> edgeValues = new();
            await dataContext.VertexEntities.AddAsync(edgeEntity);
            await dataContext.SaveChangesAsync();
            using (var reader = new StringReader(dataFile))
            {
                var headerLine = reader.ReadLine();
                if (headerLine == null) return false;

                var headers = headerLine.Split("\",\"");
                headers[0] = headers[0].Substring(1);
                if (headers.Length != 0)
                {
                    var lastWords = headers[headers.Length - 1];
                    headers[headers.Length - 1] = lastWords.Substring(0, lastWords.Length - 1);
                }

                foreach (var att in headers) edgeAttributes.Add(new VertexAttribute(att, edgeEntity.Id));

                foreach (var attribute in edgeAttributes) await dataContext.VertexAttributes.AddAsync(attribute);

                await dataContext.SaveChangesAsync();
                string? line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var objectId = Guid.NewGuid().ToString();
                    var values = line.Split("\",\"");

                    values[0] = values[0].Substring(1);
                    if (values.Length != 0)
                    {
                        var lastWord = values[values.Length - 1];
                        values[values.Length - 1] = lastWord.Substring(0, lastWord.Length - 1);
                    }

                    for (var i = 0; i < values.Length; i++)
                        edgeValues.Add(new VertexValue(values[i], edgeAttributes[i].Id, objectId));
                }
            }

            foreach (var value in edgeValues) await dataContext.VertexValues.AddAsync(value);

            await dataContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
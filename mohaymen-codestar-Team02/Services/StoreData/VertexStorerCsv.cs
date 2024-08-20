using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;

namespace mohaymen_codestar_Team02.Services.StoreData;

public class VertexStorerCsv(DataContext dataContext) : IVertexStorer
{
    public async Task<bool> StoreFileData(string entityName, string dataFile, string dataGroupId)
    {
        try
        {
            VertexEntity edgeEntity = new VertexEntity(entityName, dataGroupId);
            List<VertexAttribute> edgeAttributes = new List<VertexAttribute>();
            List<VertexValue> edgeValues = new List<VertexValue>();
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
                    edgeAttributes.Add(new VertexAttribute(att, edgeEntity.Id));
                }

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var objectId = Guid.NewGuid().ToString();
                    var values = line.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        edgeValues.Add(new VertexValue(values[i], edgeAttributes[i].Id, objectId));
                    }
                }
            }

            await dataContext.VertexEntities.AddAsync(edgeEntity);
            foreach (var attribute in edgeAttributes)
            {
                await dataContext.VertexAttributes.AddAsync(attribute);
            }

            foreach (var value in edgeValues)
            {
                await dataContext.VertexValues.AddAsync(value);
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
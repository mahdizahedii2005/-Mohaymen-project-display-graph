using mohaymen_codestar_Team02.Services.ModelData.Abstraction;

namespace mohaymen_codestar_Team02.Services.ModelData;

public class CsvModelDataHandler( IEdgeModelCreator edgeModelCreator, IVertexModelCreator vertexModelCreator) : IModelHandler
{
    public IEdgeModelCreator EdgeModelCreator { get; set; } = edgeModelCreator;
    public IVertexModelCreator VertexModelCreator { get; set; } = vertexModelCreator;
    
}
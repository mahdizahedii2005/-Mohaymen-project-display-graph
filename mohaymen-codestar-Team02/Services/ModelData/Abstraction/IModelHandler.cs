namespace mohaymen_codestar_Team02.Services.ModelData.Abstraction;

public interface IModelHandler
{
    public IEdgeModelCreator EdgeModelCreator { get; set; }

    public IVertexModelCreator VertexModelCreator { get; set; }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.VertexEAV;
using mohaymen_codestar_Team02.Services;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies;

public class VertexServiceTest
{
    private IServiceProvider _serviceProvider;
    private VertexService _sut;
    private readonly IMapper _mapper;

    public VertexServiceTest()
    {
        _mapper = Substitute.For<IMapper>();
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new DataContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _sut = new VertexService(_serviceProvider, _mapper);
    }

    [Fact]
    public void GetVertexAttribute_ShouldReturnAllAttributes_WhenGivenCorrectVertexId()
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        long vertexEntityId = 1;
        var AttName1 = "Att1";
        var AttName2 = "Att2";

        var expected = new List<GetAttributeDto>()
        {
            new()
            {
                Id = 1,
                Name = AttName1
            },
            new()
            {
                Id = 2,
                Name = AttName2
            }
        };

        var dataset = new DataGroup("Dataset1", 1)
        {
            VertexEntity = new VertexEntity("Account", 1)
            {
                VertexAttributes = new List<VertexAttribute>
                {
                    new(AttName1, 1)
                    {
                        Id = 1
                    },
                    new(AttName2, 1)
                    {
                        Id = 2
                    }
                }
            }
        };

        context.Add(dataset);
        context.SaveChanges();

        _mapper.Map<GetAttributeDto>(Arg.Is<VertexAttribute>(value => value.Id == 1))
            .Returns(new GetAttributeDto()
            {
                Id = 1,
                Name = AttName1
            });

        _mapper.Map<GetAttributeDto>(Arg.Is<VertexAttribute>(value => value.Id == 2))
            .Returns(new GetAttributeDto()
            {
                Id = 2,
                Name = AttName2
            });

        // Act
        var actual = _sut.GetVertexAttributes(vertexEntityId);

        // Assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void GetVertexDetails_ReturnsCorrectDetails()
    {
        //Arange
        var objectId1 = "object1";
        var objectId2 = "object2";
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var att1 = new VertexAttribute("att1", 1);
        var att2 = new VertexAttribute("att2", 2);
        att1.Id = 1;
        att2.Id = 2;

        List<VertexAttribute> att = new() { att1, att2 };

        foreach (var attribute in att) mockContext.Add(attribute);

        var val1 = new VertexValue("val1", 1, objectId2);
        var val2 = new VertexValue("val2", 1, objectId1);
        var val3 = new VertexValue("val3", 2, objectId1);
        var val4 = new VertexValue("val4", 2, objectId2);
        List<VertexValue> vertexValues = new() { val1, val2, val3, val4 };
        foreach (var value in vertexValues) mockContext.Add(value);

        mockContext.SaveChanges();
        var expected = new Dictionary<string, string>();
        expected[att1.Name] = val2.StringValue;
        expected[att2.Name] = val3.StringValue;
        //Action
        var result = _sut.GetVertexDetails(objectId1);
        //assert
        Assert.Equal(result.AttributeValue, expected);
    }

    [Fact]
    public void GetAllVertices_ShouldReturnAllVertices_WhenGivenCorrectDatasetName()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        long datasetId = 1;
        string attName1 = "AttName1";
        string attName2 = "AttName2";
        string vertexIdentifierFieldName = attName1;

        var dataset = new DataGroup("DatasetName1", 1)
        {
            DataGroupId = 1,
            VertexEntity = new VertexEntity("Account", 1)
            {
                VertexAttributes = new List<VertexAttribute>
                {
                    new VertexAttribute(attName1, 1)
                    {
                        VertexValues = new List<VertexValue>
                        {
                            new VertexValue("val1", 1, "id1"){ VertexAttribute = new VertexAttribute(attName1, 1)},
                            new VertexValue("val2", 1, "id2") { VertexAttribute = new VertexAttribute(attName1, 1)}
                        }
                    },
                    new VertexAttribute(attName2, 1)
                    {
                        VertexValues = new List<VertexValue>
                        {
                            new VertexValue("val3", 2, "id1"){ VertexAttribute = new VertexAttribute(attName2, 1)},
                            new VertexValue("val4", 2, "id2") { VertexAttribute = new VertexAttribute(attName2, 1)}
                       }
                    }
                }
            }
        };

        mockContext.DataSets.Add(dataset);
        mockContext.SaveChanges();

        Dictionary<string, Dictionary<string, string>> expected = new Dictionary<string, Dictionary<string, string>>()
        {
            {"id1", new Dictionary<string, string>()
            {
                {attName1, "val1"},
                {attName2, "val3"}
            }}
        };

        var attValue = new Dictionary<string, string>()
        {
            {attName1, "val1"},
            //{attName2, "val3"},
        };

        // Act
        var actual = _sut.GetAllVertices(datasetId, vertexIdentifierFieldName, attValue);

        // Assert
        Assert.Equivalent(expected, actual);
    }
}
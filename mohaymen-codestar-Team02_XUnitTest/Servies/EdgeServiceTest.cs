using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Services;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies;

public class EdgeServiceTest
{
    private IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private EdgeService _sut;

    public EdgeServiceTest()
    {
        _mapper = Substitute.For<IMapper>();
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new DataContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _sut = new EdgeService(_serviceProvider, _mapper);
    }

    [Fact]
    public void GetEdgeDetails_ReturnsCorrectDetails()
    {
        //Arange
        var objectId1 = "object1";
        var objectId2 = "object2";
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var att1 = new EdgeAttribute("att1", 1);
        var att2 = new EdgeAttribute("att2", 2);
        att1.Id = 1;
        att2.Id = 2;

        List<EdgeAttribute> att = new List<EdgeAttribute>() { att1, att2 };

        foreach (var attribute in att)
        {
            mockContext.Add(attribute);
        }

        var val1 = new EdgeValue("val1", 1, objectId2);
        var val2 = new EdgeValue("val2", 1, objectId1);
        var val3 = new EdgeValue("val3", 2, objectId1);
        var val4 = new EdgeValue("val4", 2, objectId2);
        List<EdgeValue> vertexValues = new List<EdgeValue>() { val1, val2, val3, val4 };
        foreach (var value in vertexValues)
        {
            mockContext.Add(value);
        }

        mockContext.SaveChanges();
        var expected = new Dictionary<string, string>();
        expected[att1.Name] = val2.StringValue;
        expected[att2.Name] = val3.StringValue;
        //Action
        var result = _sut.GetEdgeDetails(objectId1);
        //assert
        Assert.Equal(result.AttributeValue, expected);
    }
    
    [Fact]
    public void GetEdgeAttribute_ShouldReturnAllAttributes_WhenGivenCorrectEdgeId()
    {
        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        long edgeEntityId = 1;
        var AttName1 = "Att1";
        var AttName2 = "Att2";

        var expected = new List<GetAttributeDto>()
        {
            new GetAttributeDto()
            {
                Id = 1,
                Name = AttName1
            },
            new GetAttributeDto()
            {
                Id = 2,
                Name = AttName2
            }
        };
        
        var dataset = new DataGroup("Dataset1", 1)
        {
            DataGroupId = 1,
            EdgeEntity = new EdgeEntity("Transaction", 1)
            {
                EdgeAttributes = new List<EdgeAttribute>
                {
                    new EdgeAttribute(AttName1, edgeEntityId)
                    {
                        Id = 1
                    },
                    new EdgeAttribute(AttName2, edgeEntityId)
                    {
                        Id = 2
                    }
                }
            }
        };

        context.Add(dataset);
        context.SaveChanges();

        _mapper.Map<GetAttributeDto>(Arg.Is<EdgeAttribute>(value=>value.Id == 1))
            .Returns(new GetAttributeDto()
            {
                Id = 1,
                Name = AttName1
            });
        
        _mapper.Map<GetAttributeDto>(Arg.Is<EdgeAttribute>(value=>value.Id == 2))
            .Returns(new GetAttributeDto()
            {
                Id = 2,
                Name = AttName2
            });

        // Act
        var actual = _sut.GetEdgeAttributes(edgeEntityId);
        
        // Assert
        Assert.Equivalent(expected, actual);
    }
    
     [Fact]
    public void GetAllEdges_ShouldReturnAllEdges_WhenGivenCorrectDatasetAndIdentifiersName()
    {
        using var scope = _serviceProvider.CreateScope();
        var contex = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        long datasetId = 1;
        string attName1 = "SourceAcount";
        string attName2 = "DestiantionAccount";
        string attName3 = "RandomAtt";
        string vertexIdentifierFieldName = "CardID";
        string sourceEdgeIdentifierFieldName = attName1;
        string destinationEdgeIdentifierFieldName = attName2;

        var attValue = new Dictionary<string, string>()
        {
            {attName1, "val1"},
            {attName2, "val8"}
        };

        var dataset = new DataGroup("Dataset1", 1)
        {
            EdgeEntity = new EdgeEntity("Transaction", 1)
            {
                EdgeAttributes = new List<EdgeAttribute>
                {
                    new EdgeAttribute(attName1, 1)
                    {
                        EdgeValues = new List<EdgeValue>
                        {
                            new EdgeValue("val1", 1, "id1"){ EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val1", 1, "id2") { EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val2", 1, "id3") { EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val2", 1, "id4") { EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val5", 1, "id5") { EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val1", 1, "id6") { EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val1", 1, "id7") { EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val5", 1, "id8") { EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val1", 1, "id9") { EdgeAttribute = new EdgeAttribute(attName1, 1)},
                            new EdgeValue("val3", 1, "id10") { EdgeAttribute = new EdgeAttribute(attName1, 1)}
                        }
                    }, // 2, 6, 7
                    new EdgeAttribute(attName2, 1)
                    {
                        EdgeValues = new List<EdgeValue>
                        {
                            new EdgeValue("val7", 2, "id1"){ EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val8", 2, "id2") { EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val9", 2, "id3") { EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val10", 2, "id4") { EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val11", 2, "id5") { EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val8", 2, "id6") { EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val8", 2, "id7") { EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val12", 2, "id8") { EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val12", 2, "id9") { EdgeAttribute = new EdgeAttribute(attName2, 1)},
                            new EdgeValue("val8", 2, "id10") { EdgeAttribute = new EdgeAttribute(attName2, 1)}
                        }
                    },
                    new EdgeAttribute(attName3, 1)
                    {
                        EdgeValues = new List<EdgeValue>
                        {
                            new EdgeValue("val7", 2, "id1"){ EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val11", 2, "id2") { EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val9", 2, "id3") { EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val10", 2, "id4") { EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val11", 2, "id5") { EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val8", 2, "id6") { EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val18", 2, "id7") { EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val12", 2, "id8") { EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val12", 2, "id9") { EdgeAttribute = new EdgeAttribute(attName3, 1)},
                            new EdgeValue("val8", 2, "id10") { EdgeAttribute = new EdgeAttribute(attName3, 1)}
                        }
                    }
                }
            }
        };

        var expected = new Dictionary<string, Dictionary<string, string>>()
        {
            {"id2", new Dictionary<string, string>()
            {
                {attName1, "val1"},
                {attName2, "val8"},
                {attName3, "val11"}
            }},
            {"id6", new Dictionary<string, string>()
            {
                {attName1, "val1"},
                {attName2, "val8"},
                {attName3, "val8"}
            }},
            {"id7", new Dictionary<string, string>()
            {
                {attName1, "val1"},
                {attName2, "val8"},
                {attName3, "val18"}
            }}
        };
        
        contex.DataSets.Add(dataset);
        contex.SaveChanges();

        
        // Act
        var actual = _sut.GetAllEdges(datasetId, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName, attValue);

        // Assert
        Assert.Equivalent(expected, actual);
    }
}
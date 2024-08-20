using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.StoreData;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.StorData
{
    public class VertexStorerCsvTests
    {
        private VertexStorerCsv _sut;
        private DataContext _dataContext;

        public VertexStorerCsvTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;
            _dataContext = new DataContext(options);
            _sut = new VertexStorerCsv(_dataContext);
        }

        [Fact]
        public async Task StoreFileData_ShouldReturnTrue_WhenDataIsValid()
        {
            // Arrange
            string entityName = "TestEntity";
            string dataFile = "attribute1,attribute2\nvalue1,value2\nlol1,lol2";
            string dataGroupId = "group1";

            // Act
            var result = await _sut.StoreFileData(entityName, dataFile, dataGroupId);

            // Assert
            Assert.True(result);
            Assert.Equal(1, await _dataContext.VertexEntities.CountAsync());
            Assert.Equal(2, await _dataContext.VertexAttributes.CountAsync());
            Assert.Equal(4, await _dataContext.VertexValues.CountAsync());
        }

        [Fact]
        public async Task StoreFileData_ShouldReturnFalse_WhenHeaderLineIsNull()
        {
            // Arrange
            string entityName = "TestEntity";
            string dataFile = ""; 
            string dataGroupId = "group1";

            // Act
            var result = await _sut.StoreFileData(entityName, dataFile, dataGroupId);

            // Assert
            Assert.False(result);
            Assert.Equal(0, await _dataContext.VertexEntities.CountAsync());
            Assert.Equal(0, await _dataContext.VertexAttributes.CountAsync());
            Assert.Equal(0, await _dataContext.VertexValues.CountAsync());
        }

        [Fact]
        public async Task StoreFileData_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            // Arrange
            _sut = new VertexStorerCsv(null);
            string entityName = "TestEntity";
            string dataFile = "attribute1,attribute2\nvalue1,value2";
            string dataGroupId = "group1";

            // Act
            var result = await _sut.StoreFileData(entityName, dataFile, dataGroupId);

            // Assert
            Assert.False(result);
        }
    }
}

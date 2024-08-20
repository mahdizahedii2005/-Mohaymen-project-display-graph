using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Services.StoreData;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.StorData
{
    public class EdgeStorerCsvTests
    {
        private EdgeStorerCsv _sut;
        private DataContext _dataContext;

        public EdgeStorerCsvTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dataContext = new DataContext(options);
            _sut = new EdgeStorerCsv(_dataContext);
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
            Assert.Equal(1, await _dataContext.EdgeEntities.CountAsync());
            Assert.Equal(2, await _dataContext.EdgeAttributes.CountAsync());
            Assert.Equal(4, await _dataContext.EdgeValues.CountAsync());
        }

        [Fact]
        public async Task StoreFileData_ShouldReturnFalse_WhenHeaderLineIsNull()
        {
            // Arrange
            string entityName = "TestEntity";
            string dataFile = ""; // No headers in the file
            string dataGroupId = "group1";

            // Act
            var result = await _sut.StoreFileData(entityName, dataFile, dataGroupId);

            // Assert
            Assert.False(result);
            Assert.Equal(0, await _dataContext.EdgeEntities.CountAsync());
            Assert.Equal(0, await _dataContext.EdgeAttributes.CountAsync());
            Assert.Equal(0, await _dataContext.EdgeValues.CountAsync());
        }

        [Fact]
        public async Task StoreFileData_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            // Arrange
            _sut = new EdgeStorerCsv(null);
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
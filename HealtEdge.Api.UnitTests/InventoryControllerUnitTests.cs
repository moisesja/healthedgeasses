using System.Collections.Generic;
using HealthEdgeApi.Controllers;
using HealthEdgeApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Linq;
using System;

namespace HealtEdge.Api.UnitTests
{
    
    public class InventoryControllerUnitTests
    {
        private readonly Mock<ILogger<InventoryController>> _loggerMock;

        public InventoryControllerUnitTests()
        {
            _loggerMock = new();
        }

        [Fact]
        public void GetAllUnitTest()
        {
            var controller = new InventoryController(_loggerMock.Object);

            var actionResult = controller.Get();

            // // Compare and cast. Expect no errors.
            var okResult = Assert.IsType<OkObjectResult>(actionResult);

            var list = Assert.IsAssignableFrom<ICollection<InventoryItem>>(
                okResult.Value);

            Assert.Equal(3, list.Count);
            Assert.Contains(list, _ => _.Name.Equals("Oranges"));
            Assert.Contains(list, _ => _.Name.Equals("Apples"));
            Assert.Contains(list, _ => _.Name.Equals("Pomegranates"));
        }

        [Fact]
        public void GetByNameUnitTest()
        {
            var controller = new InventoryController(_loggerMock.Object);

            var actionResult = controller.GetByName("aPpLeS");

            // Compare and cast. Expect no errors.
            var okResult = Assert.IsType<OkObjectResult>(actionResult);

            // Must exist
            Assert.NotNull(okResult.Value);

            var item = Assert.IsAssignableFrom<InventoryItem>(
                okResult.Value);

            Assert.Equal("Apples", item.Name);
            Assert.Equal(3, item.Quantity);
            Assert.Equal(DateTime.Parse("2020-01-01"), item.CreatedOn);
        }
    }
}
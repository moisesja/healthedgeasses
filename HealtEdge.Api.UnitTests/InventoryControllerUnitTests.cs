using System;
using System.Collections.Generic;
using System.Linq;
using HealthEdgeApi.Controllers;
using HealthEdgeApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

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

        [Fact]
        public void GetByNameNotFoundUnitTest()
        {
            var controller = new InventoryController(_loggerMock.Object);

            var actionResult = controller.GetByName("dummy");

            // Compare and cast. Expect an error.
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void SuccessfulInsertPutUnitTest()
        {
            var controller = new InventoryController(_loggerMock.Object);

            IActionResult actionResult = controller.Put(new InventoryItem()
            {
                Name = "Mangos", Quantity = 100, CreatedOn = DateTime.Now
            });

            // Compare and cast. Expect an error.
            var acceptedResult = Assert.IsType<AcceptedResult>(actionResult);

            var okResult = Assert.IsType<OkObjectResult>(controller.Get());

            var list = Assert.IsAssignableFrom<ICollection<InventoryItem>>(
                okResult.Value);

            Assert.Equal(4, list.Count);
            Assert.Contains(list, _ => _.Name.Equals("Oranges"));
            Assert.Contains(list, _ => _.Name.Equals("Apples"));
            Assert.Contains(list, _ => _.Name.Equals("Pomegranates"));
            Assert.Contains(list, _ => _.Name.Equals("Mangos"));
        }

        [Fact]
        public void SuccessfulUpdatePutUnitTest()
        {
            var controller = new InventoryController(_loggerMock.Object);

            IActionResult actionResult = controller.Put(new InventoryItem()
            {
                Name = "Mangos",
                Quantity = 200,
                CreatedOn = DateTime.Now
            });

            // Compare and cast. Expect an error.
            var acceptedResult = Assert.IsType<AcceptedResult>(actionResult);

            var okResult = Assert.IsType<OkObjectResult>(controller.Get());

            var list = Assert.IsAssignableFrom<ICollection<InventoryItem>>(
                okResult.Value);

            Assert.Contains(list, _ => _.Name.Equals("Mangos"));
            var item = list.Single(_ => _.Name.Equals("Mangos"));
            Assert.Equal(200, item.Quantity);
        }

        [Fact]
        public void GetByMostPopularUnitTest()
        {
            var controller = new InventoryController(_loggerMock.Object);

            var actionResult = controller.Get(null, QueryOptions.MostActivity);

            // Compare and cast. Expect no errors.
            var okResult = Assert.IsType<OkObjectResult>(actionResult);

            var list = Assert.IsAssignableFrom<ICollection<InventoryItem>>(
                okResult.Value);

            Assert.Contains(list, _ => _.Name.Equals("Mangos"));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PhishingMinds.Server.Controllers;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Xunit;

namespace PhishingMinds.Tests
{
    public class CargoControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<DbConnectionFactory> _mockFactory;

        public CargoControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockFactory = new Mock<DbConnectionFactory>(_mockConfig.Object);
        }

        [Fact]
        public void Get_ReturnsAllCargos()
        {
            // Arrange
            var expectedCargos = new List<Cargo>
            {
                new Cargo { IdCargo = 1, Nm_Cargo = "Analista" },
                new Cargo { IdCargo = 2, Nm_Cargo = "Gerente" }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("SELECT IdCargo, Nm_Cargo FROM Cargo"))
                {
                    return expectedCargos;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualCargos = Assert.IsAssignableFrom<IEnumerable<Cargo>>(okResult.Value);
            Assert.Equal(2, actualCargos.Count());
            Assert.Equal("Analista", actualCargos.First().Nm_Cargo);
        }

        [Fact]
        public void GetById_ReturnsCargo_WhenExists()
        {
            // Arrange
            var expectedCargo = new Cargo { IdCargo = 1, Nm_Cargo = "Analista" };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("SELECT IdCargo, Nm_Cargo FROM Cargo WHERE IdCargo = @Id"))
                {
                    var dict = (Dictionary<string, object>)parameters;
                    if (Convert.ToInt32(dict["Id"]) == 1 || Convert.ToInt32(dict["@Id"]) == 1)
                        return expectedCargo;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualCargo = Assert.IsType<Cargo>(okResult.Value);
            Assert.Equal(1, actualCargo.IdCargo);
            Assert.Equal("Analista", actualCargo.Nm_Cargo);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                return null; // QueryFirstOrDefault returns null
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void Create_InsertsCargo_AndReturnsCreated()
        {
            // Arrange
            var novoCargo = new Cargo { Nm_Cargo = "Diretor" };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("INSERT INTO Cargo"))
                {
                    return 5; // Return newly inserted ID (ExecuteScalar)
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.Create(novoCargo);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var actualCargo = Assert.IsType<Cargo>(createdResult.Value);
            Assert.Equal(5, actualCargo.IdCargo);
            Assert.Equal("Diretor", actualCargo.Nm_Cargo);
        }

        [Fact]
        public void Create_ReturnsBadRequest_WhenCargoIsNull()
        {
            // Arrange
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.Create(null);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public void Update_ModifiesCargo_AndReturnsNoContent()
        {
            // Arrange
            var cargoAtualizado = new Cargo { Nm_Cargo = "Analista Senior" };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("UPDATE Cargo"))
                {
                    return 1; // 1 row affected (Execute)
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.Update(1, cargoAtualizado);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Update_ReturnsNotFound_WhenCargoDoesNotExist()
        {
            // Arrange
            var cargoAtualizado = new Cargo { Nm_Cargo = "Analista Senior" };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                return 0; // 0 rows affected
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.Update(99, cargoAtualizado);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_RemovesCargo_AndReturnsNoContent()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("DELETE FROM Cargo"))
                {
                    return 1; // 1 row affected
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenCargoDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                return 0; // 0 rows affected
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new CargoController(_mockFactory.Object);

            // Act
            var result = controller.Delete(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

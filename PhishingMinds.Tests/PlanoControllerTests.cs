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
    public class PlanoControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<DbConnectionFactory> _mockFactory;

        public PlanoControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockFactory = new Mock<DbConnectionFactory>(_mockConfig.Object);
        }

        [Fact]
        public void Get_ReturnsAllPlanos()
        {
            // Arrange
            var expectedPlanos = new List<Plano>
            {
                new Plano { IdPlano = 1, Nm_Plano = "Starter", Value_Plano = 99.90m },
                new Plano { IdPlano = 2, Nm_Plano = "Enterprise", Value_Plano = 999.90m }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("FROM Plano"))
                {
                    return expectedPlanos;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualPlanos = Assert.IsAssignableFrom<IEnumerable<Plano>>(okResult.Value);
            Assert.Equal(2, actualPlanos.Count());
        }

        [Fact]
        public void Get_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockFactory.Setup(f => f.CreateConnection()).Throws(new Exception("Database connection error"));
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void GetById_ReturnsPlano_WhenExists()
        {
            // Arrange
            var expectedPlano = new Plano { IdPlano = 1, Nm_Plano = "Starter" };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("WHERE IdPlano = @Id"))
                {
                    return expectedPlano;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualPlano = Assert.IsType<Plano>(okResult.Value);
            Assert.Equal("Starter", actualPlano.Nm_Plano);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => null);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.GetById(99);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Plano não encontrado", notFoundResult.Value);
        }

        [Fact]
        public void GetPlanoDaEmpresa_ReturnsPlano_WhenExists()
        {
            // Arrange
            var expectedPlano = new Plano { IdPlano = 2, Nm_Plano = "Enterprise" };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("WHERE IdPlano = @Id"))
                {
                    return expectedPlano;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.GetPlanoDaEmpresa(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualPlano = Assert.IsType<Plano>(okResult.Value);
            Assert.Equal("Enterprise", actualPlano.Nm_Plano);
        }

        [Fact]
        public void GetPlanoDaEmpresa_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => null);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.GetPlanoDaEmpresa(99);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Plano não encontrado", notFoundResult.Value);
        }

        [Fact]
        public void Create_InsertsPlano_AndReturnsOk()
        {
            // Arrange
            var novoPlano = new Plano { Nm_Plano = "Premium", Value_Plano = 199.90m };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("INSERT INTO Plano"))
                {
                    return 3;
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.Create(novoPlano);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualPlano = Assert.IsType<Plano>(okResult.Value);
            Assert.Equal(3, actualPlano.IdPlano);
        }

        [Fact]
        public void Update_ModifiesPlano_AndReturnsOk()
        {
            // Arrange
            var plano = new Plano { Nm_Plano = "Starter Pro", Value_Plano = 129.90m };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("UPDATE Plano"))
                {
                    return 1;
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.Update(1, plano);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualPlano = Assert.IsType<Plano>(okResult.Value);
            Assert.Equal("Starter Pro", actualPlano.Nm_Plano);
        }

        [Fact]
        public void Update_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var plano = new Plano { Nm_Plano = "Starter Pro" };
            var mockConnection = new MockDbConnection((sql, parameters) => 0);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.Update(99, plano);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_RemovesPlano_AndReturnsOk()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => 1);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Plano removido", okResult.Value);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => 0);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PlanoController(_mockFactory.Object);

            // Act
            var result = controller.Delete(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

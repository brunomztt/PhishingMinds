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
    public class SetorControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<DbConnectionFactory> _mockFactory;

        public SetorControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockFactory = new Mock<DbConnectionFactory>(_mockConfig.Object);
        }

        [Fact]
        public void Get_ReturnsAllSetores()
        {
            // Arrange
            var expectedSetores = new List<Setor>
            {
                new Setor { IdSetor = 1, Nm_Setor = "TI", IdEmpresa = 1 },
                new Setor { IdSetor = 2, Nm_Setor = "RH", IdEmpresa = 1 }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("FROM Setor s"))
                {
                    return expectedSetores;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualSetores = Assert.IsAssignableFrom<IEnumerable<Setor>>(okResult.Value);
            Assert.Equal(2, actualSetores.Count());
        }

        [Fact]
        public void GetById_ReturnsSetor_WhenExists()
        {
            // Arrange
            var expectedSetor = new Setor { IdSetor = 1, Nm_Setor = "TI", IdEmpresa = 1 };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("WHERE s.IdSetor = @Id"))
                {
                    return expectedSetor;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualSetor = Assert.IsType<Setor>(okResult.Value);
            Assert.Equal("TI", actualSetor.Nm_Setor);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => null);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetByEmpresa_ReturnsSetoresForEmpresa()
        {
            // Arrange
            var expectedSetores = new List<Setor>
            {
                new Setor { IdSetor = 1, Nm_Setor = "TI", IdEmpresa = 2 }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("WHERE s.IdEmpresa = @IdEmpresa"))
                {
                    return expectedSetores;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.GetByEmpresa(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualSetores = Assert.IsAssignableFrom<IEnumerable<Setor>>(okResult.Value);
            Assert.Single(actualSetores);
            Assert.Equal("TI", actualSetores.First().Nm_Setor);
        }

        [Fact]
        public void Create_InsertsSetor_AndReturnsOk()
        {
            // Arrange
            var novoSetor = new Setor { Nm_Setor = "Financeiro", IdEmpresa = 1 };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("INSERT INTO Setor"))
                {
                    return 3; // new IdSetor
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.Create(novoSetor);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualSetor = Assert.IsType<Setor>(okResult.Value);
            Assert.Equal(3, actualSetor.IdSetor);
        }

        [Fact]
        public void Update_ModifiesSetor_AndReturnsOk()
        {
            // Arrange
            var setor = new Setor { Nm_Setor = "Vendas", IdEmpresa = 1 };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("UPDATE Setor"))
                {
                    return 1;
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.Update(1, setor);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualSetor = Assert.IsType<Setor>(okResult.Value);
            Assert.Equal("Vendas", actualSetor.Nm_Setor);
        }

        [Fact]
        public void Update_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var setor = new Setor { Nm_Setor = "Vendas", IdEmpresa = 1 };
            var mockConnection = new MockDbConnection((sql, parameters) => 0);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.Update(99, setor);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_RemovesSetor_AndReturnsOk()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => 1);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => 0);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new SetorController(_mockFactory.Object);

            // Act
            var result = controller.Delete(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

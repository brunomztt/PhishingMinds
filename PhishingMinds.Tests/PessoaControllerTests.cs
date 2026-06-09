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
    public class PessoaControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<DbConnectionFactory> _mockFactory;

        public PessoaControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockFactory = new Mock<DbConnectionFactory>(_mockConfig.Object);
        }

        [Fact]
        public void Get_ReturnsAllPessoas()
        {
            // Arrange
            var expectedPessoas = new List<Pessoa>
            {
                new Pessoa { IdUser = 1, Nome = "Bruno" }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("FROM Pessoa p"))
                {
                    return expectedPessoas;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualPessoas = Assert.IsAssignableFrom<IEnumerable<Pessoa>>(okResult.Value);
            Assert.Single(actualPessoas);
        }

        [Fact]
        public void GetById_ReturnsPessoa_WhenExists()
        {
            // Arrange
            var expectedPessoa = new Pessoa { IdUser = 1, Nome = "Bruno" };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("WHERE p.IdUser = @Id"))
                {
                    return expectedPessoa;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualPessoa = Assert.IsType<Pessoa>(okResult.Value);
            Assert.Equal("Bruno", actualPessoa.Nome);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => null);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetByEmpresa_ReturnsPessoas()
        {
            // Arrange
            var expectedPessoas = new List<Pessoa> { new Pessoa { IdUser = 1, IdEmpresa = 2 } };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("WHERE p.IdEmpresa = @IdEmpresa"))
                {
                    return expectedPessoas;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.GetByEmpresa(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualPessoas = Assert.IsAssignableFrom<IEnumerable<Pessoa>>(okResult.Value);
            Assert.Single(actualPessoas);
        }

        [Fact]
        public void GetBySetor_ReturnsPessoas()
        {
            // Arrange
            var expectedPessoas = new List<Pessoa> { new Pessoa { IdUser = 1, IdSetor = 3 } };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("WHERE p.IdSetor = @IdSetor"))
                {
                    return expectedPessoas;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.GetBySetor(3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualPessoas = Assert.IsAssignableFrom<IEnumerable<Pessoa>>(okResult.Value);
            Assert.Single(actualPessoas);
        }

        [Fact]
        public void GetByCargo_ReturnsPessoas()
        {
            // Arrange
            var expectedPessoas = new List<Pessoa> { new Pessoa { IdUser = 1, IdCargo = 4 } };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("WHERE p.IdCargo = @IdCargo"))
                {
                    return expectedPessoas;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.GetByCargo(4);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualPessoas = Assert.IsAssignableFrom<IEnumerable<Pessoa>>(okResult.Value);
            Assert.Single(actualPessoas);
        }

        [Fact]
        public void Create_InsertsPessoa_AndReturnsCreated()
        {
            // Arrange
            var novaPessoa = new Pessoa { Nome = "Maria", Email = "maria@empresa.com" };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("INSERT INTO Pessoa"))
                {
                    return 10; // returns last insert id
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.Create(novaPessoa);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var actualPessoa = Assert.IsType<Pessoa>(createdResult.Value);
            Assert.Equal(10, actualPessoa.IdUser);
            Assert.Equal(100, actualPessoa.PhishingScore);
        }

        [Fact]
        public void Create_ReturnsBadRequest_WhenNull()
        {
            // Arrange
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.Create(null);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public void Update_ModifiesPessoa_AndReturnsNoContent()
        {
            // Arrange
            var pessoa = new Pessoa { Nome = "Maria Silva" };
            var mockConnection = new MockDbConnection((sql, parameters) => 1);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.Update(1, pessoa);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Update_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var pessoa = new Pessoa { Nome = "Maria Silva" };
            var mockConnection = new MockDbConnection((sql, parameters) => 0);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.Update(99, pessoa);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_RemovesPessoa_AndReturnsNoContent()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => 1);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) => 0);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.Delete(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetCaidosPhishing_ReturnsCaidos()
        {
            // Arrange
            var expectedCaidos = new List<dynamic> { new { Nome = "Maria", Email = "maria@empresa.com" } };
            var mockConnection = new MockDbConnection((sql, parameters) => expectedCaidos);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.GetCaidosPhishing(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotEmpty((System.Collections.IEnumerable)okResult.Value);
        }

        [Theory]
        [InlineData(2, 0, false)]
        [InlineData(3, 0, true)]
        [InlineData(3, 1, false)]
        public void NecessitaTreinamento_ReturnsCorrectFlag(int falls, int completed, bool expectedNecessita)
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("FROM PhishingCampaignTarget"))
                {
                    return falls; // COUNT(*) falls
                }
                if (sql.Contains("FROM treinamento"))
                {
                    return completed; // COUNT(*) completed
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new PessoaController(_mockFactory.Object);

            // Act
            var result = controller.NecessitaTreinamento(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var val = okResult.Value;
            var flagProp = val.GetType().GetProperty("necessitaTreinamento");
            Assert.Equal(expectedNecessita, flagProp.GetValue(val));
        }
    }
}

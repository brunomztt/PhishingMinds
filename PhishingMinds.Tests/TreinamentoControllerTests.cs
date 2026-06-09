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
    public class TreinamentoControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<DbConnectionFactory> _mockFactory;

        public TreinamentoControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockFactory = new Mock<DbConnectionFactory>(_mockConfig.Object);
        }

        [Fact]
        public void Concluir_ReturnsOkAlreadyCompleted_WhenRecordExists()
        {
            // Arrange
            var request = new ConcluirTreinamentoRequest { IdUser = 1 };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("SELECT COUNT(*)") && sql.Contains("treinamento"))
                {
                    return 1; // Already completed (ExecuteScalar)
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new TreinamentoController(_mockFactory.Object);

            // Act
            var result = controller.Concluir(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = okResult.Value;
            var messageProp = responseData.GetType().GetProperty("message");
            Assert.NotNull(messageProp);
            Assert.Equal("Treinamento já concluído.", messageProp.GetValue(responseData));
        }

        [Fact]
        public void Concluir_InsertsRecord_AndReturnsOkCompleted_WhenNew()
        {
            // Arrange
            var request = new ConcluirTreinamentoRequest { IdUser = 1 };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("SELECT COUNT(*)") && sql.Contains("treinamento"))
                {
                    return 0; // Not yet completed
                }
                if (sql.Contains("INSERT INTO treinamento"))
                {
                    return 1; // 1 row inserted (Execute)
                }
                return 0;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new TreinamentoController(_mockFactory.Object);

            // Act
            var result = controller.Concluir(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = okResult.Value;
            var messageProp = responseData.GetType().GetProperty("message");
            Assert.NotNull(messageProp);
            Assert.Equal("Treinamento concluído.", messageProp.GetValue(responseData));
        }

        [Fact]
        public void GetTreinamentosEmpresa_ReturnsTreinamentos()
        {
            // Arrange
            var expectedResults = new List<dynamic>
            {
                new { IdTreinamento = 1, DtConclusao = DateTime.Now, IdUser = 1, Nome = "Bruno", Email = "bruno@empresa.com", Nm_Setor = "TI" }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("FROM treinamento t"))
                {
                    return expectedResults;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new TreinamentoController(_mockFactory.Object);

            // Act
            var result = controller.GetTreinamentosEmpresa(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualList = Assert.IsAssignableFrom<System.Collections.IEnumerable>(okResult.Value);
            Assert.NotEmpty(actualList);
        }
    }
}

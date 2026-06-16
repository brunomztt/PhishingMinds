using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using EmailDispatcher.Infrastructure.Data;
using EmailDispatcher.Infrastructure.Email;
using EmailDispatcher.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace PhishingMinds.Tests
{
    public class EmailDispatcherTests
    {
        private IConfiguration CreateMockConfig()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s["DefaultConnection"]).Returns("Server=localhost;Database=test;Uid=root;Pwd=root;");
            mockConfig.Setup(c => c.GetSection("ConnectionStrings")).Returns(mockSection.Object);
            return mockConfig.Object;
        }

        [Fact]
        public void MySqlConnectionFactory_CreateConnection_ReturnsDbConnection()
        {
            // Arrange
            var config = CreateMockConfig();
            var factory = new MySqlConnectionFactory(config);

            // Act
            using var conn = factory.CreateConnection();

            // Assert
            Assert.NotNull(conn);
            Assert.Contains("database=test", conn.ConnectionString, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task EmailSender_Send_ThrowsInvalidOperationException_WhenSmtpFails()
        {
            // Arrange
            var sender = new EmailSender();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sender.Send(
                    "to@example.com",
                    "Subject",
                    "Body",
                    "from@example.com",
                    "Campaign",
                    "password"
                )
            );
        }

        [Fact]
        public async Task EmailSender_Send_ThrowsFormatException_WhenFromEmailIsInvalid()
        {
            // Arrange
            var sender = new EmailSender();

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(() =>
                sender.Send(
                    "to@example.com",
                    "Subject",
                    "Body",
                    "invalid-email-format",
                    "Campaign",
                    "password"
                )
            );
        }

        [Fact]
        public async Task CampaignRepository_BuscarCampanhasPendentes_ReturnsCampaignList()
        {
            // Arrange
            var expectedCampaigns = new[]
            {
                new
                {
                    IdCampaign = 12,
                    IdTemplateEmpresa = 34,
                    Subject = "Alert",
                    BodyMail = "Click here",
                    NomeCampanha = "PhishTest"
                }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                Assert.Contains("WHERE pc.Status = 'AGENDADO'", sql);
                return expectedCampaigns;
            });

            var config = CreateMockConfig();
            var factory = new Mock<MySqlConnectionFactory>(config);
            factory.Setup(f => f.CreateConnection()).Returns(mockConnection);

            var repo = new CampaignRepository(factory.Object);

            // Act
            var result = await repo.BuscarCampanhasPendentes();

            // Assert
            Assert.NotNull(result);
            var campaign = Assert.Single(result);
            Assert.Equal(12, campaign.IdCampaign);
            Assert.Equal(34, campaign.IdTemplateEmpresa);
            Assert.Equal("Alert", campaign.Subject);
            Assert.Equal("Click here", campaign.BodyMail);
            Assert.Equal("PhishTest", campaign.NomeCampanha);
        }

        [Fact]
        public async Task CampaignRepository_BuscarParametrosCampanha_ReturnsParameters()
        {
            // Arrange
            var expectedParams = new[]
            {
                new
                {
                    ParameterName = "Host",
                    ParameterValue = "google.com"
                }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                Assert.Contains("FROM TemplateParameter tp", sql);
                var dict = (Dictionary<string, object>)parameters;
                Assert.Equal(55, dict["@IdTemplateEmpresa"]);
                return expectedParams;
            });

            var config = CreateMockConfig();
            var factory = new Mock<MySqlConnectionFactory>(config);
            factory.Setup(f => f.CreateConnection()).Returns(mockConnection);

            var repo = new CampaignRepository(factory.Object);

            // Act
            var result = await repo.BuscarParametrosCampanha(55);

            // Assert
            Assert.NotNull(result);
            var param = Assert.Single(result);
            Assert.Equal("Host", param.Name);
            Assert.Equal("google.com", param.Value);
        }

        [Fact]
        public async Task CampaignRepository_BuscarUsuarios_ReturnsUsers()
        {
            // Arrange
            var expectedUsers = new[]
            {
                new
                {
                    IdTarget = 99,
                    Nome = "John Doe",
                    Email = "john@example.com"
                }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                Assert.Contains("FROM PhishingCampaignTarget t", sql);
                var dict = (Dictionary<string, object>)parameters;
                Assert.Equal(10, dict["@IdCampaign"]);
                return expectedUsers;
            });

            var config = CreateMockConfig();
            var factory = new Mock<MySqlConnectionFactory>(config);
            factory.Setup(f => f.CreateConnection()).Returns(mockConnection);

            var repo = new CampaignRepository(factory.Object);

            // Act
            var result = await repo.BuscarUsuarios(10);

            // Assert
            Assert.NotNull(result);
            var user = Assert.Single(result);
            Assert.Equal(99, user.IdTarget);
            Assert.Equal("John Doe", user.Nome);
            Assert.Equal("john@example.com", user.Email);
        }

        [Fact]
        public async Task CampaignRepository_MarcarComoEnviado_ExecutesNonQuery()
        {
            // Arrange
            bool queryExecuted = false;
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                Assert.Contains("UPDATE PhishingCampaignTarget", sql);
                var dict = (Dictionary<string, object>)parameters;
                Assert.Equal(77, dict["@IdTarget"]);
                queryExecuted = true;
                return 1;
            });

            var config = CreateMockConfig();
            var factory = new Mock<MySqlConnectionFactory>(config);
            factory.Setup(f => f.CreateConnection()).Returns(mockConnection);

            var repo = new CampaignRepository(factory.Object);

            // Act
            await repo.MarcarComoEnviado(77);

            // Assert
            Assert.True(queryExecuted);
        }

        [Fact]
        public async Task CampaignRepository_MarcarCampanhaComoProcessada_ExecutesNonQuery()
        {
            // Arrange
            bool queryExecuted = false;
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                Assert.Contains("UPDATE PhishingCampaign", sql);
                var dict = (Dictionary<string, object>)parameters;
                Assert.Equal(88, dict["@IdCampaign"]);
                queryExecuted = true;
                return 1;
            });

            var config = CreateMockConfig();
            var factory = new Mock<MySqlConnectionFactory>(config);
            factory.Setup(f => f.CreateConnection()).Returns(mockConnection);

            var repo = new CampaignRepository(factory.Object);

            // Act
            await repo.MarcarCampanhaComoProcessada(88);

            // Assert
            Assert.True(queryExecuted);
        }

        [Fact]
        public async Task CampaignRepository_GetMailCredential_ReturnsCredential_WhenExists()
        {
            // Arrange
            var expectedCredential = new[]
            {
                new
                {
                    Id_EmailCredentials = 123,
                    Mail = "admin@phishing.com",
                    Senha = "secret123"
                }
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                Assert.Contains("FROM MailCredentials", sql);
                return expectedCredential;
            });

            var config = CreateMockConfig();
            var factory = new Mock<MySqlConnectionFactory>(config);
            factory.Setup(f => f.CreateConnection()).Returns(mockConnection);

            var repo = new CampaignRepository(factory.Object);

            // Act
            var result = await repo.GetMailCredential();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(123, result.Id);
            Assert.Equal("admin@phishing.com", result.Mail);
            Assert.Equal("secret123", result.Senha);
        }

        [Fact]
        public async Task CampaignRepository_GetMailCredential_ReturnsNull_WhenDoesNotExist()
        {
            // Arrange
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                Assert.Contains("FROM MailCredentials", sql);
                return new List<object>(); // empty
            });

            var config = CreateMockConfig();
            var factory = new Mock<MySqlConnectionFactory>(config);
            factory.Setup(f => f.CreateConnection()).Returns(mockConnection);

            var repo = new CampaignRepository(factory.Object);

            // Act
            var result = await repo.GetMailCredential();

            // Assert
            Assert.Null(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmailDispatcher.Application.Services;
using EmailDispatcher.Domain.Entities;
using EmailDispatcher.Infrastructure.Email;
using EmailDispatcher.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace PhishingMinds.Tests
{
    public class CampaignServiceTests
    {
        private readonly Mock<CampaignRepository> _mockRepo;
        private readonly Mock<EmailSender> _mockEmailSender;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<CampaignService>> _mockLogger;

        public CampaignServiceTests()
        {
            // Since CampaignRepository takes MySqlConnectionFactory, we pass null to constructor as we mock the virtual methods.
            _mockRepo = new Mock<CampaignRepository>(new object[] { null });
            _mockEmailSender = new Mock<EmailSender>();
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<CampaignService>>();
        }

        [Fact]
        public async Task ProcessarCampanhas_DoesNothing_WhenNoPendingCampaigns()
        {
            // Arrange
            _mockRepo.Setup(r => r.BuscarCampanhasPendentes()).ReturnsAsync(new List<Campaign>());
            var service = new CampaignService(_mockRepo.Object, _mockEmailSender.Object, _mockConfig.Object, _mockLogger.Object);

            // Act
            await service.ProcessarCampanhas();

            // Assert
            _mockRepo.Verify(r => r.BuscarCampanhasPendentes(), Times.Once);
            _mockRepo.Verify(r => r.GetMailCredential(), Times.Never);
        }

        [Fact]
        public async Task ProcessarCampanhas_SendsEmails_AndMarksProcessed_WhenCampaignsExist()
        {
            // Arrange
            var mockCampaigns = new List<Campaign>
            {
                new Campaign { IdCampaign = 1, Subject = "Subject 1", BodyMail = "Hello {{Nome}} {{Link}}", NomeCampanha = "Camp 1" }
            };

            var mockUsers = new List<User>
            {
                new User { IdTarget = 101, Nome = "User 1", Email = "user1@test.com" }
            };

            var mockCredential = new MailCredential { Id = 1, Mail = "sender@test.com", Senha = "pwd" };

            _mockRepo.Setup(r => r.BuscarCampanhasPendentes()).ReturnsAsync(mockCampaigns);
            _mockRepo.Setup(r => r.GetMailCredential()).ReturnsAsync(mockCredential);
            _mockRepo.Setup(r => r.BuscarUsuarios(1)).ReturnsAsync(mockUsers);
            _mockRepo.Setup(r => r.MarcarComoEnviado(It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.MarcarCampanhaComoProcessada(It.IsAny<int>())).Returns(Task.CompletedTask);

            _mockEmailSender.Setup(e => e.Send(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            )).Returns(Task.CompletedTask);

            _mockConfig.Setup(c => c["TrackingBaseUrl"]).Returns("http://localhost");

            var service = new CampaignService(_mockRepo.Object, _mockEmailSender.Object, _mockConfig.Object, _mockLogger.Object);

            // Act
            await service.ProcessarCampanhas();

            // Assert
            _mockRepo.Verify(r => r.BuscarCampanhasPendentes(), Times.Once);
            _mockRepo.Verify(r => r.GetMailCredential(), Times.Once);
            _mockRepo.Verify(r => r.BuscarUsuarios(1), Times.Once);
            
            _mockEmailSender.Verify(e => e.Send(
                "user1@test.com",
                It.IsAny<string>(),
                It.Is<string>(b =>
                    b.Contains("User 1") &&
                    b.Contains("http://localhost/track?idTarget=101")
                ),
                "sender@test.com",
                It.IsAny<string>(),
                "pwd"
            ), Times.Once);

            _mockRepo.Verify(r => r.MarcarComoEnviado(101), Times.Once);
            _mockRepo.Verify(r => r.MarcarCampanhaComoProcessada(1), Times.Once);
        }
    }
}

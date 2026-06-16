using System;
using System.Collections.Generic;
using PhishingMinds.Server.Class;
using Xunit;

namespace PhishingMinds.Tests
{
    public class ModelTests
    {
        [Fact]
        public void MailCredentials_GetterAndSetter_WorkCorrectly()
        {
            // Arrange & Act
            var model = new MailCredentials
            {
                Id_EmailCredentials = 10,
                Mail = "test@example.com",
                Senha = "mypassword"
            };

            // Assert
            Assert.Equal(10, model.Id_EmailCredentials);
            Assert.Equal("test@example.com", model.Mail);
            Assert.Equal("mypassword", model.Senha);
        }

        [Fact]
        public void ParameterValueEntry_GetterAndSetter_WorkCorrectly()
        {
            // Arrange & Act
            var model = new ParameterValueEntry
            {
                IdParameterValue = 1,
                IdParameter = 2,
                IdTemplateEmpresa = 3,
                ParameterValue = "Value",
                ParameterName = "Name"
            };

            // Assert
            Assert.Equal(1, model.IdParameterValue);
            Assert.Equal(2, model.IdParameter);
            Assert.Equal(3, model.IdTemplateEmpresa);
            Assert.Equal("Value", model.ParameterValue);
            Assert.Equal("Name", model.ParameterName);
        }

        [Fact]
        public void PhishingCampaign_GetterAndSetter_WorkCorrectly()
        {
            // Arrange
            var testDate = DateTime.Now;
            var setores = new List<int> { 1, 2, 3 };

            // Act
            var model = new PhishingCampaign
            {
                IdCampaign = 100,
                IdEmpresa = 200,
                IdTemplateEmpresa = 300,
                IdSetor = 400,
                NomeCampanha = "Campaign A",
                Dt_Disparo = testDate,
                Status = "AGENDADO",
                IdSetores = setores,
                Nm_Empresa = "Empresa A",
                NomeTemplate = "Template A",
                Nm_Setor = "Setor A"
            };

            // Assert
            Assert.Equal(100, model.IdCampaign);
            Assert.Equal(200, model.IdEmpresa);
            Assert.Equal(300, model.IdTemplateEmpresa);
            Assert.Equal(400, model.IdSetor);
            Assert.Equal("Campaign A", model.NomeCampanha);
            Assert.Equal(testDate, model.Dt_Disparo);
            Assert.Equal("AGENDADO", model.Status);
            Assert.Equal(setores, model.IdSetores);
            Assert.Equal("Empresa A", model.Nm_Empresa);
            Assert.Equal("Template A", model.NomeTemplate);
            Assert.Equal("Setor A", model.Nm_Setor);
        }

        [Fact]
        public void PhishingCampaignTarget_GetterAndSetter_WorkCorrectly()
        {
            // Arrange
            var testDate = DateTime.Now;

            // Act
            var model = new PhishingCampaignTarget
            {
                IdTarget = 50,
                IdCampaign = 60,
                IdUser = 70,
                MailSent = true,
                MailOpened = true,
                LinkClicked = true,
                CredentialsSubmitted = true,
                Reported = true,
                Dt_Register = testDate,
                NomeUsuario = "User X",
                NomeCampanha = "Campaign X"
            };

            // Assert
            Assert.Equal(50, model.IdTarget);
            Assert.Equal(60, model.IdCampaign);
            Assert.Equal(70, model.IdUser);
            Assert.True(model.MailSent);
            Assert.True(model.MailOpened);
            Assert.True(model.LinkClicked);
            Assert.True(model.CredentialsSubmitted);
            Assert.True(model.Reported);
            Assert.Equal(testDate, model.Dt_Register);
            Assert.Equal("User X", model.NomeUsuario);
            Assert.Equal("Campaign X", model.NomeCampanha);
        }

        [Fact]
        public void PhishingTemplate_GetterAndSetter_WorkCorrectly()
        {
            // Arrange & Act
            var model = new PhishingTemplate
            {
                IdTemplate = 1,
                NomeTemplate = "Template Name",
                Subject = "Template Subject",
                BodyMail = "Template Body",
                Categoria = "Template Category",
                NivelDificuldade = 5
            };

            // Assert
            Assert.Equal(1, model.IdTemplate);
            Assert.Equal("Template Name", model.NomeTemplate);
            Assert.Equal("Template Subject", model.Subject);
            Assert.Equal("Template Body", model.BodyMail);
            Assert.Equal("Template Category", model.Categoria);
            Assert.Equal(5, model.NivelDificuldade);
        }

        [Fact]
        public void PhishingTemplateEmpresa_GetterAndSetter_WorkCorrectly()
        {
            // Arrange & Act
            var model = new PhishingTemplateEmpresa
            {
                IdTemplateEmpresa = 10,
                IdEmpresa = 20,
                IdTemplate = 30,
                NomePersonalizado = "Custom Name",
                NomeTemplate = "Template Name",
                Nm_Empresa = "Empresa Name"
            };

            // Assert
            Assert.Equal(10, model.IdTemplateEmpresa);
            Assert.Equal(20, model.IdEmpresa);
            Assert.Equal(30, model.IdTemplate);
            Assert.Equal("Custom Name", model.NomePersonalizado);
            Assert.Equal("Template Name", model.NomeTemplate);
            Assert.Equal("Empresa Name", model.Nm_Empresa);
        }

        [Fact]
        public void Treinamento_GetterAndSetter_WorkCorrectly()
        {
            // Arrange
            var testDate = DateTime.Now;

            // Act
            var model = new Treinamento
            {
                IdTreinamento = 1,
                IdUser = 2,
                Aprovado = true,
                DtConclusao = testDate
            };

            // Assert
            Assert.Equal(1, model.IdTreinamento);
            Assert.Equal(2, model.IdUser);
            Assert.True(model.Aprovado);
            Assert.Equal(testDate, model.DtConclusao);
        }
    }
}

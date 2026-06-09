using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PhishingMinds.Server.Controllers;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Xunit;

namespace PhishingMinds.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<DbConnectionFactory> _mockFactory;

        public AuthControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            // Setup default Configuration return values
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("PhishingMindsSuperSecretKey12345!@#");
            _mockFactory = new Mock<DbConnectionFactory>(_mockConfig.Object);
        }

        [Fact]
        public void Login_ReturnsOk_WhenEmpresaCredentialsAreValid()
        {
            // Arrange
            var request = new AuthController.LoginRequest { Email = "admin@phishingminds.com", Password = "123" };
            var mockEmpresa = new Empresa
            {
                IdEmpresa = 1,
                IdPlano = 1,
                Nm_Dono = "Bruno",
                Mail = "admin@phishingminds.com",
                Senha = "123"
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("FROM Empresa WHERE Mail = @Email"))
                {
                    return mockEmpresa;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = okResult.Value;
            var userProp = responseData.GetType().GetProperty("User");
            Assert.NotNull(userProp);
            var userVal = userProp.GetValue(responseData);
            var nomeProp = userVal.GetType().GetProperty("nome");
            Assert.Equal("Bruno", nomeProp.GetValue(userVal));
        }

        [Fact]
        public void Login_ReturnsOk_WhenPessoaCredentialsAreValid()
        {
            // Arrange
            var request = new AuthController.LoginRequest { Email = "bruno@empresa.com", Password = "123" };
            var mockPessoa = new Pessoa
            {
                IdUser = 1,
                IdEmpresa = 1,
                Nome = "Bruno",
                Email = "bruno@empresa.com",
                Senha = "123" // plaintext match or hashed match
            };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("FROM Empresa WHERE Mail = @Email"))
                {
                    return null; // No Empresa found
                }
                if (sql.Contains("FROM Pessoa WHERE Email = @Email"))
                {
                    return mockPessoa;
                }
                if (sql.Contains("SELECT IdPlano FROM Empresa"))
                {
                    return 1; // planoId
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = okResult.Value;
            var userProp = responseData.GetType().GetProperty("User");
            Assert.NotNull(userProp);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var request = new AuthController.LoginRequest { Email = "wrong@email.com", Password = "abc" };

            var mockConnection = new MockDbConnection((sql, parameters) => null);
            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Credenciais inválidas.", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var request = new AuthController.LoginRequest { Email = "test@email.com", Password = "123" };
            _mockFactory.Setup(f => f.CreateConnection()).Throws(new Exception("Database connection failed"));
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.Login(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public void ResetPassword_ReturnsOk_ForEmpresa()
        {
            // Arrange
            var request = new AuthController.ResetPasswordRequest { Email = "admin@phishingminds.com", NewPassword = "newpassword" };
            var mockEmpresa = new Empresa { Mail = "admin@phishingminds.com" };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("SELECT * FROM Empresa WHERE Mail = @Email"))
                {
                    return mockEmpresa;
                }
                if (sql.Contains("UPDATE Empresa SET Senha = @Senha"))
                {
                    return 1;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.ResetPassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Senha de Administrador atualizada com sucesso.", okResult.Value);
        }

        [Fact]
        public void ResetPassword_ReturnsOk_ForPessoa()
        {
            // Arrange
            var request = new AuthController.ResetPasswordRequest { Email = "bruno@empresa.com", NewPassword = "newpassword" };
            var mockPessoa = new Pessoa { Email = "bruno@empresa.com" };

            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("SELECT * FROM Empresa WHERE Mail = @Email"))
                {
                    return null;
                }
                if (sql.Contains("SELECT * FROM Pessoa WHERE Email = @Email"))
                {
                    return mockPessoa;
                }
                if (sql.Contains("UPDATE Pessoa SET Senha = @Senha"))
                {
                    return 1;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.ResetPassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Senha de Colaborador atualizada com sucesso.", okResult.Value);
        }

        [Fact]
        public void ResetPassword_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new AuthController.ResetPasswordRequest { Email = "nonexistent@email.com", NewPassword = "password" };
            var mockConnection = new MockDbConnection((sql, parameters) => null);

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.ResetPassword(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuário não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public void Register_CreatesEmpresa_AndReturnsOk()
        {
            // Arrange
            var request = new Empresa { Mail = "new@company.com", Nm_Empresa = "New Company" };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("SELECT * FROM Empresa WHERE Mail = @Email"))
                {
                    return null; // Email free
                }
                if (sql.Contains("INSERT INTO Empresa"))
                {
                    return 1;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = okResult.Value;
            var messageProp = responseData.GetType().GetProperty("message");
            Assert.Equal("Cadastro realizado com sucesso.", messageProp.GetValue(responseData));
        }

        [Fact]
        public void Register_ReturnsBadRequest_WhenEmailExists()
        {
            // Arrange
            var request = new Empresa { Mail = "existing@company.com" };
            var mockEmpresa = new Empresa { Mail = "existing@company.com" };
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("SELECT * FROM Empresa WHERE Mail = @Email"))
                {
                    return mockEmpresa;
                }
                return null;
            });

            _mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection);
            var controller = new AuthController(_mockConfig.Object, _mockFactory.Object);

            // Act
            var result = controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Este e-mail já está em uso.", badRequestResult.Value);
        }
    }
}

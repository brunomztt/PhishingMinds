using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PhishingMinds.Server.Controllers;
using PhishingMinds.Server.Data;
using Xunit;

namespace PhishingMinds.Tests
{
    public class DashboardControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<DbConnectionFactory> _mockFactory;

        public DashboardControllerTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockFactory = new Mock<DbConnectionFactory>(_mockConfig.Object);
        }

        [Fact]
        public void GetMetrics_DeveRetornarOk()
        {
            var mockConnection = new MockDbConnection((sql, parameters) =>
            {
                if (sql.Contains("COUNT(*) FROM PhishingCampaign"))
                    return 5;

                if (sql.Contains("COUNT(DISTINCT t.IdUser)"))
                    return 20;

                if (sql.Contains("FROM PhishingCampaignTarget pct"))
                {
                    if (sql.Contains("CredentialsSubmitted"))
                        return 10;

                    return 100;
                }

                return 0;
            });

            _mockFactory
                .Setup(f => f.CreateConnection())
                .Returns(mockConnection);

            var controller =
                new DashboardController(
                    _mockFactory.Object
                );

            var result =
                controller.GetMetrics(1);

            Assert.IsType<OkObjectResult>(
                result
            );
        }

        [Fact]
        public void GetSetoresRanking_DeveRetornarOk()
        {
            var ranking =
                new List<object>
                {
                    new
                    {
                        Setor = "TI",
                        Score = 95
                    },
                    new
                    {
                        Setor = "RH",
                        Score = 80
                    }
                };

            var mockConnection =
                new MockDbConnection(
                    (sql, parameters) =>
                    {
                        if (
                            sql.Contains(
                                "AVG(p.PhishingScore)"
                            )
                        )
                            return ranking;

                        return null;
                    }
                );

            _mockFactory
                .Setup(f => f.CreateConnection())
                .Returns(mockConnection);

            var controller =
                new DashboardController(
                    _mockFactory.Object
                );

            var result =
                controller.GetSetoresRanking(1);

            Assert.IsType<OkObjectResult>(
                result
            );
        }

        [Fact]
        public void GetEvolucao_DeveRetornarOk()
        {
            var campanhas =
                new List<dynamic>
                {
                    new
                    {
                        IdCampaign = 1,
                        NomeCampanha = "Teste",
                        Dt_Disparo = System.DateTime.Now,
                        Setores = "TI",
                        TotalUsuarios = 10,
                        LinksClicados = 2,
                        CredenciaisEnviadas = 1
                    }
                };

            var mockConnection =
                new MockDbConnection(
                    (sql, parameters) =>
                    {
                        if (
                            sql.Contains(
                                "GROUP_CONCAT"
                            )
                        )
                            return campanhas;

                        return null;
                    }
                );

            _mockFactory
                .Setup(f => f.CreateConnection())
                .Returns(mockConnection);

            var controller =
                new DashboardController(
                    _mockFactory.Object
                );

            var result =
                controller.GetEvolucao(1);

            Assert.IsType<OkObjectResult>(
                result
            );
        }

        [Fact]
        public void GetSetoresLista_DeveRetornarOk()
        {
            var setores =
                new List<object>
                {
                    new
                    {
                        IdSetor = 1,
                        Nm_Setor = "TI"
                    }
                };

            var mockConnection =
                new MockDbConnection(
                    (sql, parameters) =>
                    {
                        if (
                            sql.Contains(
                                "FROM Setor"
                            )
                        )
                            return setores;

                        return null;
                    }
                );

            _mockFactory
                .Setup(f => f.CreateConnection())
                .Returns(mockConnection);

            var controller =
                new DashboardController(
                    _mockFactory.Object
                );

            var result =
                controller.GetSetoresLista(1);

            Assert.IsType<OkObjectResult>(
                result
            );
        }

        [Fact]
        public void GetMetrics_DeveRetornarErro500()
        {
            _mockFactory
                .Setup(f => f.CreateConnection())
                .Throws(
                    new System.Exception(
                        "Erro banco"
                    )
                );

            var controller =
                new DashboardController(
                    _mockFactory.Object
                );

            var result =
                controller.GetMetrics(1);

            Assert.IsType<ObjectResult>(
                result
            );
        }
    }
}
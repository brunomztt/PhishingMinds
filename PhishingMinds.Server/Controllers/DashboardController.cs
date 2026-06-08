using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public DashboardController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet("metrics/{idEmpresa}")]
        public IActionResult GetMetrics(int idEmpresa)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                // 1. Active Campaigns
                var activeCampaigns = db.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM PhishingCampaign WHERE IdEmpresa = @IdEmpresa",
                    new { IdEmpresa = idEmpresa }
                );

                // 2. Trained Users
                var trainedUsers = db.ExecuteScalar<int>(
                    @"
                    SELECT COUNT(DISTINCT t.IdUser)
                    FROM treinamento t
                    INNER JOIN Pessoa p
                        ON p.IdUser = t.IdUser
                    WHERE p.IdEmpresa = @IdEmpresa
                      AND t.Aprovado = 1
                    ",
                    new { IdEmpresa = idEmpresa }
                );

                // 3. Failure Rate
                var totalSent = db.ExecuteScalar<int>(
                    @"SELECT COUNT(*) 
                      FROM PhishingCampaignTarget pct 
                      JOIN PhishingCampaign pc ON pct.IdCampaign = pc.IdCampaign 
                      WHERE pc.IdEmpresa = @IdEmpresa",
                    new { IdEmpresa = idEmpresa }
                );

                var failures = db.ExecuteScalar<int>(
                    @"SELECT COUNT(*) 
                      FROM PhishingCampaignTarget pct 
                      JOIN PhishingCampaign pc ON pct.IdCampaign = pc.IdCampaign 
                      WHERE pc.IdEmpresa = @IdEmpresa AND (pct.LinkClicked = 1 OR pct.CredentialsSubmitted = 1)",
                    new { IdEmpresa = idEmpresa }
                );

                double failureRate = totalSent > 0 ? ((double)failures * 100.0 / totalSent) : 0.0;

                // 4. Security Evolution (safety rate: 100 - failure rate)
                double avgEvolution = 100.0 - failureRate;

                return Ok(new
                {
                    ActiveCampaigns = activeCampaigns,
                    TrainedUsers = trainedUsers,
                    FailureRate = Math.Round(failureRate, 1),
                    AvgEvolution = Math.Round(avgEvolution, 1)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar métricas", error = ex.Message });
            }
        }

        [HttpGet("setores/{idEmpresa}")]
        public IActionResult GetSetoresRanking(int idEmpresa)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();
                var sql = @"
                    SELECT 
                        s.Nm_Setor as Setor, 
                        IFNULL(AVG(p.PhishingScore), 0) as Score
                    FROM Setor s
                    LEFT JOIN Pessoa p ON s.IdSetor = p.IdSetor
                    WHERE s.IdEmpresa = @IdEmpresa
                    GROUP BY s.IdSetor, s.Nm_Setor
                    ORDER BY Score DESC";

                var result = db.Query(sql, new { IdEmpresa = idEmpresa }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar ranking de setores", error = ex.Message });
            }
        }
    }
}

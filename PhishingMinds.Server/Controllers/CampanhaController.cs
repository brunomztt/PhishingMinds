using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CampanhaController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public CampanhaController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet("empresa/{idEmpresa}")]
        public ActionResult<IEnumerable<PhishingCampaign>> GetByEmpresa(int idEmpresa)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();
                var sql = @"
                    SELECT pc.*, e.Nm_Empresa, s.Nm_Setor
                    FROM PhishingCampaign pc
                    LEFT JOIN Empresa e ON pc.IdEmpresa = e.IdEmpresa
                    LEFT JOIN Setor s ON pc.IdSetor = s.IdSetor
                    WHERE pc.IdEmpresa = @IdEmpresa";
                var lista = db.Query<PhishingCampaign>(sql, new { IdEmpresa = idEmpresa }).ToList();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar campanhas", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Create([FromBody] PhishingCampaign novaCampanha)
        {
            if (novaCampanha == null)
                return BadRequest();

            try
            {
                using var db = _dbFactory.CreateConnection();
                novaCampanha.Dt_Disparo = DateTime.Now;
                novaCampanha.Status = "AGENDADO";

                var sql = @"
                    INSERT INTO PhishingCampaign (IdEmpresa, IdTemplateEmpresa, IdSetor, NomeCampanha, Dt_Disparo, Status)
                    VALUES (@IdEmpresa, @IdTemplateEmpresa, @IdSetor, @NomeCampanha, @Dt_Disparo, @Status);
                    SELECT LAST_INSERT_ID();";

                var id = db.ExecuteScalar<int>(sql, novaCampanha);
                novaCampanha.IdCampaign = id;

                return Ok(novaCampanha);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar campanha", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();
                var rows = db.Execute("DELETE FROM PhishingCampaign WHERE IdCampaign = @Id", new { Id = id });

                if (rows == 0)
                    return NotFound(new { message = "Campanha não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = "Erro ao excluir campanha", error = ex.Message });
            }
        }
    }
}

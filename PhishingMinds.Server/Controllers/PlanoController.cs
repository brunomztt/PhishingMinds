using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Dapper;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Descomente quando implementar autenticação
    public class PlanoController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public PlanoController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet]
        public ActionResult<List<Plano>> Get()
        {
            try
            {
                using var db = _dbFactory.CreateConnection();
                var planos = db.Query<Plano>("SELECT IdPlano, Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns FROM Plano").ToList();
                return Ok(planos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar os planos", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Plano> GetById(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var plano = db.QueryFirstOrDefault<Plano>("SELECT IdPlano, Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns FROM Plano WHERE IdPlano = @Id", new { Id = id });

            if (plano == null)
                return NotFound("Plano não encontrado");

            return Ok(plano);
        }

        [HttpGet("{id}/plano")]
        public ActionResult GetPlanoDaEmpresa(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var plano = db.QueryFirstOrDefault<Plano>("SELECT IdPlano, Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns FROM Plano WHERE IdPlano = @Id", new { Id = id });

            if (plano == null)
                return NotFound("Plano não encontrado");

            return Ok(plano);
        }

        [HttpPost]
        public ActionResult Create(Plano novoPlano)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                INSERT INTO Plano (Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns) 
                VALUES (@Nm_Plano, @Desc_Plano, @Temp_Plano, @Value_Plano, @MaxUsers, @MaxCampaigns);
                SELECT LAST_INSERT_ID();";

            var id = db.ExecuteScalar<int>(sql, novoPlano);
            novoPlano.IdPlano = id;

            return Ok(novoPlano);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Plano planoAtualizado)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                UPDATE Plano 
                SET Nm_Plano = @Nm_Plano, Desc_Plano = @Desc_Plano, Temp_Plano = @Temp_Plano, 
                    Value_Plano = @Value_Plano, MaxUsers = @MaxUsers, MaxCampaigns = @MaxCampaigns
                WHERE IdPlano = @Id";

            var rows = db.Execute(sql, new { 
                planoAtualizado.Nm_Plano, 
                planoAtualizado.Desc_Plano, 
                planoAtualizado.Temp_Plano, 
                planoAtualizado.Value_Plano, 
                planoAtualizado.MaxUsers, 
                planoAtualizado.MaxCampaigns, 
                Id = id 
            });

            if (rows == 0)
                return NotFound("Plano não encontrado");

            planoAtualizado.IdPlano = id;
            return Ok(planoAtualizado);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var rows = db.Execute("DELETE FROM Plano WHERE IdPlano = @Id", new { Id = id });

            if (rows == 0)
                return NotFound("Plano não encontrado");

            return Ok("Plano removido");
        }
    }
}
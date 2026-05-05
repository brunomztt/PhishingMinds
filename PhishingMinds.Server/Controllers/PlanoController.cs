using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Dapper;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanoController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public PlanoController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Plano>> Get()
        {
            List<Plano> planos = new List<Plano>();

            try
            {
                string query = "SELECT IdPlano, Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns FROM Plano";
                //Vou criar um CoreDb pra facilitar a conexão com o banco, mas por enquanto, aqui está a estrutura básica para buscar os planos do banco de dados:

                // Simulando conexao ante do CoreDb
                planos.Add(new Plano { IdPlano = 1, Nm_Plano = "Ouro", Desc_Plano = "Plano inicial", Temp_Plano = 12, Value_Plano = 99.90m, MaxUsers = 50, MaxCampaigns = 5 });
                planos.Add(new Plano { IdPlano = 2, Nm_Plano = "Diamante", Desc_Plano = "Plano completo", Temp_Plano = 12, Value_Plano = 999.90m, MaxUsers = 1000, MaxCampaigns = 999 });
                planos.Add(new Plano { IdPlano = 3, Nm_Plano = "Prata", Desc_Plano = "Plano Intermediario", Temp_Plano = 12, Value_Plano = 499.90m, MaxUsers = 500, MaxCampaigns = 499 });

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
            var plano = db.QueryFirstOrDefault<Plano>("SELECT IdPlano as Id_Plano, Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns FROM Plano WHERE IdPlano = @Id", new { Id = id });

            if (plano == null)
                return NotFound("Plano não encontrado");

            return Ok(plano);
        }

        [HttpGet("{id}/plano")]
        public ActionResult GetPlanoDaEmpresa(int id)
        {
            // Note: the original code just fetched by Id_Plano. If this meant to fetch the plano of an empresa, it should join.
            // Leaving as is to match original behavior, which is basically GetById.
            using var db = _dbFactory.CreateConnection();
            var plano = db.QueryFirstOrDefault<Plano>("SELECT IdPlano as Id_Plano, Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns FROM Plano WHERE IdPlano = @Id", new { Id = id });

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
            novoPlano.Id_Plano = id;

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

            planoAtualizado.Id_Plano = id;
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
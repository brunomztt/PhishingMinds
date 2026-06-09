using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Dapper;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public CargoController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Cargo>> Get()
        {
            using var db = _dbFactory.CreateConnection();
            var cargos = db.Query<Cargo>("SELECT IdCargo, Nm_Cargo FROM Cargo").ToList();
            return Ok(cargos);
        }

        [HttpGet("{id}")]
        public ActionResult<Cargo> GetById(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var cargo = db.QueryFirstOrDefault<Cargo>("SELECT IdCargo, Nm_Cargo FROM Cargo WHERE IdCargo = @Id", new { Id = id });
            
            if (cargo == null)
                return NotFound(new { message = "Cargo não encontrado" });

            return Ok(cargo);
        }

        [HttpPost]
        public ActionResult<Cargo> Create([FromBody] Cargo novoCargo)
        {
            if (novoCargo == null)
                return BadRequest();

            using var db = _dbFactory.CreateConnection();
            var sql = @"INSERT INTO Cargo (Nm_Cargo) VALUES (@Nm_Cargo); SELECT LAST_INSERT_ID();";
            
            var id = db.ExecuteScalar<int>(sql, novoCargo);
            novoCargo.IdCargo = id;

            return CreatedAtAction(nameof(GetById), new { id = novoCargo.IdCargo }, novoCargo);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Cargo cargoAtualizado)
        {
            using var db = _dbFactory.CreateConnection();
            var rows = db.Execute("UPDATE Cargo SET Nm_Cargo = @Nm_Cargo WHERE IdCargo = @Id", new { Nm_Cargo = cargoAtualizado.Nm_Cargo, Id = id });
            
            if (rows == 0)
                return NotFound(new { message = "Cargo não encontrado" });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var rows = db.Execute("DELETE FROM Cargo WHERE IdCargo = @Id", new { Id = id });
            
            if (rows == 0)
                return NotFound(new { message = "Cargo não encontrado" });

            return NoContent();
        }
    }
}
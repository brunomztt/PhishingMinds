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
    public class SetorController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public SetorController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet]
        public ActionResult<List<Setor>> Get()
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                SELECT s.*, p.Nome as Nm_Gestor 
                FROM Setor s
                LEFT JOIN Pessoa p ON s.IdGestor = p.IdUser";
            var setores = db.Query<Setor>(sql).ToList();
            return Ok(setores);
        }

        [HttpGet("{id}")]
        public ActionResult<Setor> GetById(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                SELECT s.*, p.Nome as Nm_Gestor 
                FROM Setor s
                LEFT JOIN Pessoa p ON s.IdGestor = p.IdUser
                WHERE s.IdSetor = @Id";
            var setor = db.QueryFirstOrDefault<Setor>(sql, new { Id = id });

            if (setor == null)
                return NotFound("Setor não encontrado");

            return Ok(setor);
        }

        [HttpGet("empresa/{idEmpresa}")]
        public ActionResult<List<Setor>> GetByEmpresa(int idEmpresa)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                SELECT s.*, p.Nome as Nm_Gestor 
                FROM Setor s
                LEFT JOIN Pessoa p ON s.IdGestor = p.IdUser
                WHERE s.IdEmpresa = @IdEmpresa";
            var lista = db.Query<Setor>(sql, new { IdEmpresa = idEmpresa }).ToList();
            return Ok(lista);
        }

        [HttpPost]
        public ActionResult Create(Setor novoSetor)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                INSERT INTO Setor (IdEmpresa, Nm_Setor, IdGestor) 
                VALUES (@IdEmpresa, @Nm_Setor, @IdGestor);
                SELECT LAST_INSERT_ID();";

            var id = db.ExecuteScalar<int>(sql, novoSetor);
            novoSetor.IdSetor = id;

            return Ok(novoSetor);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Setor setorAtualizado)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                UPDATE Setor 
                SET Nm_Setor = @Nm_Setor, IdEmpresa = @IdEmpresa, IdGestor = @IdGestor
                WHERE IdSetor = @Id";

            setorAtualizado.IdSetor = id;
            var rows = db.Execute(sql, setorAtualizado);

            if (rows == 0)
                return NotFound("Setor não encontrado");

            return Ok(setorAtualizado);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var rows = db.Execute("DELETE FROM Setor WHERE IdSetor = @Id", new { Id = id });

            if (rows == 0)
                return NotFound("Setor não encontrado");

            return Ok(new { message = "Setor removido" });
        }
    }
}
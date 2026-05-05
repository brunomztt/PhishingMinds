using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PessoaController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public PessoaController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Pessoa>> Get()
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                SELECT p.*, s.Nm_Setor, c.Nm_Cargo, g.Nome as Nm_Gestor
                FROM Pessoa p
                LEFT JOIN Setor s ON p.IdSetor = s.IdSetor
                LEFT JOIN Cargo c ON p.IdCargo = c.IdCargo
                LEFT JOIN Pessoa g ON s.IdGestor = g.IdUser";
            var pessoas = db.Query<Pessoa>(sql).ToList();
            return Ok(pessoas);
        }

        [HttpGet("{id}")]
        public ActionResult<Pessoa> GetById(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                SELECT p.*, s.Nm_Setor, c.Nm_Cargo, g.Nome as Nm_Gestor
                FROM Pessoa p
                LEFT JOIN Setor s ON p.IdSetor = s.IdSetor
                LEFT JOIN Cargo c ON p.IdCargo = c.IdCargo
                LEFT JOIN Pessoa g ON s.IdGestor = g.IdUser
                WHERE p.IdUser = @Id";
            var pessoa = db.QueryFirstOrDefault<Pessoa>(sql, new { Id = id });

            if (pessoa == null)
                return NotFound(new { message = "Pessoa não encontrada" });

            return Ok(pessoa);
        }

        [HttpGet("empresa/{idEmpresa}")]
        public ActionResult<IEnumerable<Pessoa>> GetByEmpresa(int idEmpresa)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                SELECT p.*, s.Nm_Setor, c.Nm_Cargo, g.Nome as Nm_Gestor
                FROM Pessoa p
                LEFT JOIN Setor s ON p.IdSetor = s.IdSetor
                LEFT JOIN Cargo c ON p.IdCargo = c.IdCargo
                LEFT JOIN Pessoa g ON s.IdGestor = g.IdUser
                WHERE p.IdEmpresa = @IdEmpresa";
            var lista = db.Query<Pessoa>(sql, new { IdEmpresa = idEmpresa }).ToList();
            return Ok(lista);
        }

        [HttpGet("setor/{idSetor}")]
        public ActionResult<IEnumerable<Pessoa>> GetBySetor(int idSetor)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                SELECT p.*, s.Nm_Setor, c.Nm_Cargo, g.Nome as Nm_Gestor
                FROM Pessoa p
                LEFT JOIN Setor s ON p.IdSetor = s.IdSetor
                LEFT JOIN Cargo c ON p.IdCargo = c.IdCargo
                LEFT JOIN Pessoa g ON s.IdGestor = g.IdUser
                WHERE p.IdSetor = @IdSetor";
            var lista = db.Query<Pessoa>(sql, new { IdSetor = idSetor }).ToList();
            return Ok(lista);
        }

        [HttpGet("cargo/{idCargo}")]
        public ActionResult<IEnumerable<Pessoa>> GetByCargo(int idCargo)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"
                SELECT p.*, s.Nm_Setor, c.Nm_Cargo, g.Nome as Nm_Gestor
                FROM Pessoa p
                LEFT JOIN Setor s ON p.IdSetor = s.IdSetor
                LEFT JOIN Cargo c ON p.IdCargo = c.IdCargo
                LEFT JOIN Pessoa g ON s.IdGestor = g.IdUser
                WHERE p.IdCargo = @IdCargo";
            var lista = db.Query<Pessoa>(sql, new { IdCargo = idCargo }).ToList();
            return Ok(lista);
        }

        [HttpPost]
        public ActionResult<Pessoa> Create([FromBody] Pessoa novaPessoa)
        {
            if (novaPessoa == null)
                return BadRequest();

            using var db = _dbFactory.CreateConnection();
            novaPessoa.Dt_cadastro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            novaPessoa.Senha = HashPassword(Guid.NewGuid().ToString("N").Substring(0, 8)); // Generate a random hash code for the initial password

            var sql = @"
                INSERT INTO Pessoa (IdEmpresa, IdSetor, IdCargo, Nome, Email, Senha, Ativo, Dt_cadastro, UltimoLogin, PhishingScore)
                VALUES (@IdEmpresa, @IdSetor, @IdCargo, @Nome, @Email, @Senha, @Ativo, @Dt_cadastro, @UltimoLogin, @PhishingScore);
                SELECT LAST_INSERT_ID();";

            var id = db.ExecuteScalar<int>(sql, novaPessoa);
            novaPessoa.IdUser = id;

            return CreatedAtAction(nameof(GetById), new { id = novaPessoa.IdUser }, novaPessoa);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Pessoa pessoaAtualizada)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();
                
                var sql = @"
                    UPDATE Pessoa 
                    SET Nome = @Nome, Email = @Email, Ativo = @Ativo, 
                        PhishingScore = @PhishingScore, IdEmpresa = @IdEmpresa, 
                        IdSetor = @IdSetor, IdCargo = @IdCargo
                    WHERE IdUser = @IdUser";

                pessoaAtualizada.IdUser = id;
                var rows = db.Execute(sql, pessoaAtualizada);

                if (rows == 0)
                    return NotFound(new { message = "Pessoa não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar usuário", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();
                var rows = db.Execute("DELETE FROM Pessoa WHERE IdUser = @Id", new { Id = id });

                if (rows == 0)
                    return NotFound(new { message = "Pessoa não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = "Erro ao excluir. O usuário pode estar vinculado a outros registros.", error = ex.Message });
            }
        }
    }
}
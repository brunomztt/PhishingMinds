using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Data;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TreinamentoController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public TreinamentoController(
            DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpPost("concluir")]
        public IActionResult Concluir(
            [FromBody] ConcluirTreinamentoRequest request)
        {
            using var db = _dbFactory.CreateConnection();

            var jaConcluiu = db.ExecuteScalar<int>(
                @"
                SELECT COUNT(*)
                FROM treinamento
                WHERE IdUser = @IdUser
                AND Aprovado = 1
                ",
                new { request.IdUser }
            );

            if (jaConcluiu > 0)
            {
                return Ok(new
                {
                    message = "Treinamento já concluído."
                });
            }

            db.Execute(
                @"
                INSERT INTO treinamento
                (
                    IdUser,
                    Aprovado,
                    DtConclusao
                )
                VALUES
                (
                    @IdUser,
                    1,
                    NOW()
                )
                ",
                new { request.IdUser }
            );

            return Ok(new
            {
                message = "Treinamento concluído."
            });
        }

        [HttpGet("empresa/{idEmpresa}")]
        public IActionResult GetTreinamentosEmpresa(int idEmpresa)
        {
            using var db = _dbFactory.CreateConnection();

            var sql = @"
        SELECT
            t.IdTreinamento,
            t.DtConclusao,
            p.IdUser,
            p.Nome,
            p.Email,
            s.Nm_Setor
        FROM treinamento t
        INNER JOIN pessoa p
            ON p.IdUser = t.IdUser
        LEFT JOIN setor s
            ON s.IdSetor = p.IdSetor
        WHERE p.IdEmpresa = @IdEmpresa
          AND t.Aprovado = 1
        ORDER BY t.DtConclusao DESC
    ";

            var resultado = db.Query(sql, new
            {
                IdEmpresa = idEmpresa
            });

            return Ok(resultado);
        }
    }

    public class ConcluirTreinamentoRequest
    {
        public int IdUser { get; set; }
    }
}
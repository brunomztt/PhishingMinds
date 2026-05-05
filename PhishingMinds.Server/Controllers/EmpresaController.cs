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
    public class EmpresaController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public EmpresaController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet]
        public ActionResult<List<Empresa>> Get()
        {
            using var db = _dbFactory.CreateConnection();
            var empresas = db.Query<Empresa>("SELECT * FROM Empresa").ToList();
            return Ok(empresas);
        }

        [HttpGet("{id}")]
        public ActionResult<Empresa> GetById(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var empresa = db.QueryFirstOrDefault<Empresa>("SELECT * FROM Empresa WHERE IdEmpresa = @Id", new { Id = id });

            if (empresa == null)
                return NotFound("Empresa não encontrada");

            return Ok(empresa);
        }

        [HttpPost]
        public ActionResult Create(Empresa novaEmpresa)
        {
            using var db = _dbFactory.CreateConnection();
            novaEmpresa.Dt_Cadastro = DateTime.Now;

            var sql = @"INSERT INTO Empresa (IdPlano, Nm_Empresa, Nm_Dono, Mail, CNPJ, Dt_Cadastro, Dt_Contratacao, Dt_FimContrato, Ativo) 
                        VALUES (@IdPlano, @Nm_Empresa, @Nm_Dono, @Mail, @CNPJ, @Dt_Cadastro, @Dt_Contratacao, @Dt_FimContrato, @Ativo);
                        SELECT LAST_INSERT_ID();";

            var id = db.ExecuteScalar<int>(sql, novaEmpresa);
            novaEmpresa.IdEmpresa = id;

            return Ok(novaEmpresa);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Empresa empresaAtualizada)
        {
            using var db = _dbFactory.CreateConnection();
            var sql = @"UPDATE Empresa 
                        SET IdPlano = @IdPlano, Nm_Empresa = @Nm_Empresa, Nm_Dono = @Nm_Dono, Mail = @Mail, 
                            CNPJ = @CNPJ, Dt_Contratacao = @Dt_Contratacao, Dt_FimContrato = @Dt_FimContrato, Ativo = @Ativo
                        WHERE IdEmpresa = @IdEmpresa";

            empresaAtualizada.IdEmpresa = id;
            var rows = db.Execute(sql, empresaAtualizada);

            if (rows == 0)
                return NotFound("Empresa não encontrada");

            return Ok(empresaAtualizada);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            using var db = _dbFactory.CreateConnection();
            var rows = db.Execute("DELETE FROM Empresa WHERE IdEmpresa = @Id", new { Id = id });

            if (rows == 0)
                return NotFound("Empresa não encontrada");

            return Ok("Empresa removida");
        }
    }
}
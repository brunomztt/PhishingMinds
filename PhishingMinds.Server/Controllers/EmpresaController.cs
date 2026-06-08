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

        [HttpGet("global-contracts")]
        public IActionResult GetGlobalContracts()
        {
            try
            {
                using var db = _dbFactory.CreateConnection();
                var activeCount = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Empresa WHERE Ativo = 1");
                var mrr = db.ExecuteScalar<decimal>("SELECT IFNULL(SUM(p.Value_Plano), 0) FROM Empresa e JOIN Plano p ON e.IdPlano = p.IdPlano WHERE e.Ativo = 1");
                var list = db.Query(@"
                    SELECT e.Nm_Empresa AS nm_Empresa, p.Nm_Plano AS nm_Plano, p.Value_Plano AS value_Plano, e.Ativo AS ativo 
                    FROM Empresa e 
                    JOIN Plano p ON e.IdPlano = p.IdPlano").ToList();
                return Ok(new { ActiveCompanies = activeCount, MRR = mrr, Subscriptions = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar contratos globais", error = ex.Message });
            }
        }

        [HttpGet("billing/{id}")]
        public IActionResult GetBillingInfo(int id)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();
                var plano = db.QueryFirstOrDefault<Plano>(@"
                    SELECT p.* FROM Empresa e 
                    JOIN Plano p ON e.IdPlano = p.IdPlano 
                    WHERE e.IdEmpresa = @Id", new { Id = id });

                if (plano == null)
                    return NotFound("Plano não encontrado para esta empresa");

                var activeEmployees = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Pessoa WHERE IdEmpresa = @Id AND Ativo = 1", new { Id = id });
                var campaignCount = db.ExecuteScalar<int>("SELECT COUNT(*) FROM PhishingCampaign WHERE IdEmpresa = @Id", new { Id = id });

                return Ok(new
                {
                    Plano = plano,
                    ActiveEmployees = activeEmployees,
                    CampaignCount = campaignCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar faturamento da empresa", error = ex.Message });
            }
        }
    }
}
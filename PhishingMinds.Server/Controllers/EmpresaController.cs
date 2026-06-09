using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private bool IsSha256(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length != 64)
                return false;

            return Regex.IsMatch(password, "^[a-fA-F0-9]{64}$");
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
            novaEmpresa.Dt_Contratacao = DateTime.Now;
            novaEmpresa.Dt_FimContrato = DateTime.Now.AddYears(1);
            novaEmpresa.Ativo = true;

            if (string.IsNullOrEmpty(novaEmpresa.Senha))
            {
                novaEmpresa.Senha = "1";
            }
            novaEmpresa.Senha = HashPassword(novaEmpresa.Senha);

            var sql = @"INSERT INTO Empresa (IdPlano, Nm_Empresa, Nm_Dono, Mail, Senha, CNPJ, Dt_Cadastro, Dt_Contratacao, Dt_FimContrato, Ativo) 
                        VALUES (@IdPlano, @Nm_Empresa, @Nm_Dono, @Mail, @Senha, @CNPJ, @Dt_Cadastro, @Dt_Contratacao, @Dt_FimContrato, @Ativo);
                        SELECT LAST_INSERT_ID();";

            var id = db.ExecuteScalar<int>(sql, novaEmpresa);
            novaEmpresa.IdEmpresa = id;

            return Ok(novaEmpresa);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Empresa empresaAtualizada)
        {
            using var db = _dbFactory.CreateConnection();
            
            var existing = db.QueryFirstOrDefault<Empresa>("SELECT Senha, Dt_Cadastro FROM Empresa WHERE IdEmpresa = @Id", new { Id = id });
            if (existing == null)
                return NotFound("Empresa não encontrada");

            if (string.IsNullOrEmpty(empresaAtualizada.Senha))
            {
                empresaAtualizada.Senha = existing.Senha;
            }
            else if (!IsSha256(empresaAtualizada.Senha))
            {
                empresaAtualizada.Senha = HashPassword(empresaAtualizada.Senha);
            }

            empresaAtualizada.Dt_Cadastro = existing.Dt_Cadastro;
            empresaAtualizada.IdEmpresa = id;

            var sql = @"UPDATE Empresa 
                        SET IdPlano = @IdPlano, Nm_Empresa = @Nm_Empresa, Nm_Dono = @Nm_Dono, Mail = @Mail, Senha = @Senha,
                            CNPJ = @CNPJ, Dt_Contratacao = @Dt_Contratacao, Dt_FimContrato = @Dt_FimContrato, Ativo = @Ativo
                        WHERE IdEmpresa = @IdEmpresa";

            var rows = db.Execute(sql, empresaAtualizada);

            if (rows == 0)
                return NotFound("Empresa não encontrada");

            return Ok(empresaAtualizada);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            using var db = _dbFactory.CreateConnection();
            db.Open();
            using var transaction = db.BeginTransaction();
            try
            {
                // 1. Break sector-manager cyclic dependency
                db.Execute("UPDATE Setor SET IdGestor = NULL WHERE IdEmpresa = @Id", new { Id = id }, transaction);
                db.Execute("UPDATE Pessoa SET IdSetor = NULL WHERE IdEmpresa = @Id", new { Id = id }, transaction);

                // 2. Delete training records
                db.Execute("DELETE FROM treinamento WHERE IdUser IN (SELECT IdUser FROM Pessoa WHERE IdEmpresa = @Id)", new { Id = id }, transaction);

                // 3. Delete campaign targets
                db.Execute("DELETE FROM PhishingCampaignTarget WHERE IdUser IN (SELECT IdUser FROM Pessoa WHERE IdEmpresa = @Id) OR IdCampaign IN (SELECT IdCampaign FROM PhishingCampaign WHERE IdEmpresa = @Id)", new { Id = id }, transaction);

                // 4. Delete campaign sectors
                db.Execute("DELETE FROM PhishingCampaignSetor WHERE IdCampaign IN (SELECT IdCampaign FROM PhishingCampaign WHERE IdEmpresa = @Id)", new { Id = id }, transaction);

                // 5. Delete campaigns
                db.Execute("DELETE FROM PhishingCampaign WHERE IdEmpresa = @Id", new { Id = id }, transaction);

                // 6. Delete template parameter values
                db.Execute("DELETE FROM ParameterValue WHERE IdTemplateEmpresa IN (SELECT IdTemplateEmpresa FROM PhishingTemplateEmpresa WHERE IdEmpresa = @Id)", new { Id = id }, transaction);

                // 7. Delete custom templates
                db.Execute("DELETE FROM PhishingTemplateEmpresa WHERE IdEmpresa = @Id", new { Id = id }, transaction);

                // 8. Delete people
                db.Execute("DELETE FROM Pessoa WHERE IdEmpresa = @Id", new { Id = id }, transaction);

                // 9. Delete sectors
                db.Execute("DELETE FROM Setor WHERE IdEmpresa = @Id", new { Id = id }, transaction);

                // 10. Delete company
                var rows = db.Execute("DELETE FROM Empresa WHERE IdEmpresa = @Id", new { Id = id }, transaction);

                if (rows == 0)
                {
                    transaction.Rollback();
                    return NotFound("Empresa não encontrada");
                }

                transaction.Commit();
                return Ok(new { message = "Empresa e todos os seus vínculos removidos com sucesso." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(500, new { message = "Erro ao excluir empresa", error = ex.Message });
            }
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
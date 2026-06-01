using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;
using PhishingMinds.Server.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        private (int idEmpresa, bool isDevAdmin) GetUserContext()
        {
            var idEmpresaClaim = User.FindFirst("IdEmpresa")?.Value;
            int.TryParse(idEmpresaClaim, out var idEmpresa);
            return (idEmpresa, idEmpresa == 1);
        }

        [HttpGet("empresa/{idEmpresa}")]
        public ActionResult<IEnumerable<PhishingCampaign>> GetByEmpresa(int idEmpresa)
        {
            try
            {
                var context = GetUserContext();
                using var db = _dbFactory.CreateConnection();
                
                string sql;
                List<PhishingCampaign> lista;

                // Dev Admin (idEmpresa == 1) visualiza todas as campanhas de todas as empresas do sistema
                if (idEmpresa == 1 || context.isDevAdmin)
                {
                    sql = @"
                        SELECT pc.*, e.Nm_Empresa, COALESCE(pte.NomePersonalizado, pt.NomeTemplate) as NomeTemplate
                        FROM PhishingCampaign pc
                        LEFT JOIN Empresa e ON pc.IdEmpresa = e.IdEmpresa
                        LEFT JOIN PhishingTemplateEmpresa pte ON pc.IdTemplateEmpresa = pte.IdTemplateEmpresa
                        LEFT JOIN PhishingTemplate pt ON pte.IdTemplate = pt.IdTemplate";
                    lista = db.Query<PhishingCampaign>(sql).ToList();
                }
                else
                {
                    sql = @"
                        SELECT pc.*, e.Nm_Empresa, COALESCE(pte.NomePersonalizado, pt.NomeTemplate) as NomeTemplate
                        FROM PhishingCampaign pc
                        LEFT JOIN Empresa e ON pc.IdEmpresa = e.IdEmpresa
                        LEFT JOIN PhishingTemplateEmpresa pte ON pc.IdTemplateEmpresa = pte.IdTemplateEmpresa
                        LEFT JOIN PhishingTemplate pt ON pte.IdTemplate = pt.IdTemplate
                        WHERE pc.IdEmpresa = @IdEmpresa";
                    lista = db.Query<PhishingCampaign>(sql, new { IdEmpresa = idEmpresa }).ToList();
                }

                // Buscar múltiplos setores para cada campanha
                foreach (var c in lista)
                {
                    var sectors = db.Query<(int IdSetor, string Nm_Setor)>(@"
                        SELECT pcs.IdSetor, s.Nm_Setor 
                        FROM PhishingCampaignSetor pcs
                        JOIN Setor s ON pcs.IdSetor = s.IdSetor
                        WHERE pcs.IdCampaign = @IdCampaign", new { IdCampaign = c.IdCampaign }).ToList();
                    
                    c.IdSetores = sectors.Select(x => x.IdSetor).ToList();
                    c.Nm_Setor = sectors.Any() ? string.Join(", ", sectors.Select(x => x.Nm_Setor)) : "Geral (Todos os Setores)";
                }

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
                return BadRequest("Dados da campanha inválidos.");

            try
            {
                var context = GetUserContext();
                using var db = _dbFactory.CreateConnection();

                // Se não for Dev Admin, força o IdEmpresa a ser a empresa do usuário logado
                if (!context.isDevAdmin)
                {
                    novaCampanha.IdEmpresa = context.idEmpresa;
                }

                // Se Dt_Disparo for padrão ou não informada, assume agora
                if (novaCampanha.Dt_Disparo == default)
                {
                    novaCampanha.Dt_Disparo = DateTime.Now;
                }

                // Definir status com base na data de disparo
                novaCampanha.Status = novaCampanha.Dt_Disparo > DateTime.Now ? "AGENDADO" : "DISPARADO";

                // Definir IdSetor da tabela antiga para o primeiro elemento (compatibilidade de BD)
                novaCampanha.IdSetor = novaCampanha.IdSetores != null && novaCampanha.IdSetores.Any() 
                    ? (int?)novaCampanha.IdSetores.First() 
                    : null;

                var sql = @"
                    INSERT INTO PhishingCampaign (IdEmpresa, IdTemplateEmpresa, IdSetor, NomeCampanha, Dt_Disparo, Status)
                    VALUES (@IdEmpresa, @IdTemplateEmpresa, @IdSetor, @NomeCampanha, @Dt_Disparo, @Status);
                    SELECT LAST_INSERT_ID();";

                var id = db.ExecuteScalar<int>(sql, novaCampanha);
                novaCampanha.IdCampaign = id;

                // Salvar múltiplos setores em PhishingCampaignSetor
                if (novaCampanha.IdSetores != null)
                {
                    foreach (var idSetor in novaCampanha.IdSetores)
                    {
                        db.Execute(@"
                            INSERT INTO PhishingCampaignSetor (IdCampaign, IdSetor)
                            VALUES (@IdCampaign, @IdSetor)", new { IdCampaign = id, IdSetor = idSetor });
                    }
                }

                // Criar automaticamente alvos em PhishingCampaignTarget
                List<int> targetUserIds;
                if (novaCampanha.IdSetores != null && novaCampanha.IdSetores.Any())
                {
                    targetUserIds = db.Query<int>(@"
                        SELECT IdUser FROM Pessoa 
                        WHERE IdEmpresa = @IdEmpresa AND IdSetor IN @IdSetores AND Ativo = 1",
                        new { IdEmpresa = novaCampanha.IdEmpresa, IdSetores = novaCampanha.IdSetores }).ToList();
                }
                else
                {
                    targetUserIds = db.Query<int>(@"
                        SELECT IdUser FROM Pessoa 
                        WHERE IdEmpresa = @IdEmpresa AND Ativo = 1",
                        new { IdEmpresa = novaCampanha.IdEmpresa }).ToList();
                }

                foreach (var userId in targetUserIds)
                {
                    db.Execute(@"
                        INSERT INTO PhishingCampaignTarget (IdCampaign, IdUser, MailSent, MailOpened, LinkClicked, CredentialsSubmitted, Reported)
                        VALUES (@IdCampaign, @IdUser, 0, 0, 0, 0, 0)",
                        new { IdCampaign = id, IdUser = userId });
                }

                return Ok(novaCampanha);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar campanha", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] PhishingCampaign campanhaAtualizada)
        {
            if (campanhaAtualizada == null)
                return BadRequest("Dados de atualização inválidos.");

            try
            {
                var context = GetUserContext();
                using var db = _dbFactory.CreateConnection();

                // 1. Obter a campanha existente no banco
                var sqlExisting = "SELECT * FROM PhishingCampaign WHERE IdCampaign = @Id";
                var existing = db.QueryFirstOrDefault<PhishingCampaign>(sqlExisting, new { Id = id });

                if (existing == null)
                    return NotFound(new { message = "Campanha não encontrada." });

                // 2. Segurança: Apenas a própria empresa ou o Dev Admin podem editar
                if (!context.isDevAdmin && existing.IdEmpresa != context.idEmpresa)
                    return Forbid("Você não tem permissão para editar esta campanha.");

                // 3. Regra de Negócio: Empresa normal só pode editar ANTES da data de disparo
                if (!context.isDevAdmin && existing.Dt_Disparo <= DateTime.Now)
                {
                    return BadRequest(new { message = "Não é possível editar uma campanha cujo disparo já passou ou está ocorrendo." });
                }

                // 4. Se a campanha for editada por empresa comum, o IdEmpresa deve ser mantido
                var idEmpresaToSave = context.isDevAdmin ? campanhaAtualizada.IdEmpresa : existing.IdEmpresa;

                // Definir novo status com base na nova data
                var novoStatus = campanhaAtualizada.Dt_Disparo > DateTime.Now ? "AGENDADO" : "DISPARADO";

                // Definir IdSetor primário ( BD antigo )
                var primarySetorId = campanhaAtualizada.IdSetores != null && campanhaAtualizada.IdSetores.Any() 
                    ? (int?)campanhaAtualizada.IdSetores.First() 
                    : null;

                var sqlUpdate = @"
                    UPDATE PhishingCampaign
                    SET IdEmpresa = @IdEmpresa,
                        IdTemplateEmpresa = @IdTemplateEmpresa,
                        IdSetor = @IdSetor,
                        NomeCampanha = @NomeCampanha,
                        Dt_Disparo = @Dt_Disparo,
                        Status = @Status
                    WHERE IdCampaign = @IdCampaign";

                db.Execute(sqlUpdate, new
                {
                    IdEmpresa = idEmpresaToSave,
                    IdTemplateEmpresa = campanhaAtualizada.IdTemplateEmpresa,
                    IdSetor = primarySetorId,
                    NomeCampanha = campanhaAtualizada.NomeCampanha,
                    Dt_Disparo = campanhaAtualizada.Dt_Disparo,
                    Status = novoStatus,
                    IdCampaign = id
                });

                // Atualizar setores na tabela de múltiplos setores
                db.Execute("DELETE FROM PhishingCampaignSetor WHERE IdCampaign = @IdCampaign", new { IdCampaign = id });
                if (campanhaAtualizada.IdSetores != null)
                {
                    foreach (var idSetor in campanhaAtualizada.IdSetores)
                    {
                        db.Execute(@"
                            INSERT INTO PhishingCampaignSetor (IdCampaign, IdSetor)
                            VALUES (@IdCampaign, @IdSetor)", new { IdCampaign = id, IdSetor = idSetor });
                    }
                }

                // Atualizar alvos na tabela PhishingCampaignTarget
                db.Execute("DELETE FROM PhishingCampaignTarget WHERE IdCampaign = @IdCampaign", new { IdCampaign = id });
                
                List<int> targetUserIds;
                if (campanhaAtualizada.IdSetores != null && campanhaAtualizada.IdSetores.Any())
                {
                    targetUserIds = db.Query<int>(@"
                        SELECT IdUser FROM Pessoa 
                        WHERE IdEmpresa = @IdEmpresa AND IdSetor IN @IdSetores AND Ativo = 1",
                        new { IdEmpresa = idEmpresaToSave, IdSetores = campanhaAtualizada.IdSetores }).ToList();
                }
                else
                {
                    targetUserIds = db.Query<int>(@"
                        SELECT IdUser FROM Pessoa 
                        WHERE IdEmpresa = @IdEmpresa AND Ativo = 1",
                        new { IdEmpresa = idEmpresaToSave }).ToList();
                }

                foreach (var userId in targetUserIds)
                {
                    db.Execute(@"
                        INSERT INTO PhishingCampaignTarget (IdCampaign, IdUser, MailSent, MailOpened, LinkClicked, CredentialsSubmitted, Reported)
                        VALUES (@IdCampaign, @IdUser, 0, 0, 0, 0, 0)",
                        new { IdCampaign = id, IdUser = userId });
                }

                campanhaAtualizada.IdCampaign = id;
                campanhaAtualizada.Status = novoStatus;
                if (!context.isDevAdmin)
                {
                    campanhaAtualizada.IdEmpresa = existing.IdEmpresa;
                }

                return Ok(campanhaAtualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar campanha", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var context = GetUserContext();
                using var db = _dbFactory.CreateConnection();

                // 1. Obter a campanha existente
                var existing = db.QueryFirstOrDefault<PhishingCampaign>(
                    "SELECT * FROM PhishingCampaign WHERE IdCampaign = @Id", new { Id = id });

                if (existing == null)
                    return NotFound(new { message = "Campanha não encontrada" });

                // 2. Segurança: Apenas a própria empresa ou o Dev Admin podem excluir
                if (!context.isDevAdmin && existing.IdEmpresa != context.idEmpresa)
                    return Forbid("Você não tem permissão para excluir esta campanha.");

                // 3. Excluir alvos associados em PhishingCampaignTarget
                db.Execute("DELETE FROM PhishingCampaignTarget WHERE IdCampaign = @Id", new { Id = id });

                // 4. Excluir setores associados em PhishingCampaignSetor
                db.Execute("DELETE FROM PhishingCampaignSetor WHERE IdCampaign = @IdCampaign", new { IdCampaign = id });

                // 5. Excluir a campanha
                db.Execute("DELETE FROM PhishingCampaign WHERE IdCampaign = @Id", new { Id = id });

                return Ok(new { message = "Campanha excluída com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir campanha", error = ex.Message });
            }
        }
    }
}

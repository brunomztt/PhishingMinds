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
    public class TemplateController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public TemplateController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public class CustomizeRequest
        {
            public int IdTemplate { get; set; }
            public string NomePersonalizado { get; set; }
            public List<ParameterValueInput> Parameters { get; set; }
        }

        public class ParameterValueInput
        {
            public int IdParameter { get; set; }
            public string ParameterValue { get; set; }
        }

        [HttpGet("empresa/{idEmpresa}")]
        public ActionResult GetByEmpresa(int idEmpresa)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                // 1. Verificar se a empresa tem templates associados
                var checkSql = "SELECT COUNT(*) FROM PhishingTemplateEmpresa WHERE IdEmpresa = @IdEmpresa";
                var count = db.ExecuteScalar<int>(checkSql, new { IdEmpresa = idEmpresa });

                if (count == 0)
                {
                    // Seed automático: associar todos os templates base existentes para esta empresa
                    var baseTemplates = db.Query<PhishingTemplate>("SELECT * FROM PhishingTemplate").ToList();
                    foreach (var bt in baseTemplates)
                    {
                        var insertPteSql = @"
                            INSERT INTO PhishingTemplateEmpresa (IdEmpresa, IdTemplate, NomePersonalizado)
                            VALUES (@IdEmpresa, @IdTemplate, @NomePersonalizado);
                            SELECT LAST_INSERT_ID();";

                        var newId = db.ExecuteScalar<int>(insertPteSql, new
                        {
                            IdEmpresa = idEmpresa,
                            IdTemplate = bt.IdTemplate,
                            NomePersonalizado = bt.NomeTemplate + " Customizado"
                        });

                        // Seed dos valores dos parâmetros padrão
                        var baseParams = db.Query<TemplateParameter>(
                            "SELECT * FROM TemplateParameter WHERE IdTemplate = @IdTemplate",
                            new { IdTemplate = bt.IdTemplate }
                        ).ToList();

                        foreach (var bp in baseParams)
                        {
                            var defaultValue = bp.ParameterName.ToLower() == "link" 
                                ? "https://seguranca-phishingminds.com/login" 
                                : bp.ExampleValue;

                            db.Execute(@"
                                INSERT INTO ParameterValue (IdParameter, IdTemplateEmpresa, ParameterValue)
                                VALUES (@IdParameter, @IdTemplateEmpresa, @ParameterValue)",
                                new { IdParameter = bp.IdParameter, IdTemplateEmpresa = newId, ParameterValue = defaultValue });
                        }
                    }
                }

                // 2. Buscar templates associados à empresa
                var templatesSql = @"
                    SELECT 
                        pte.IdTemplateEmpresa,
                        pte.IdEmpresa,
                        pte.IdTemplate,
                        pte.NomePersonalizado,
                        pt.NomeTemplate,
                        pt.Subject,
                        pt.BodyMail,
                        pt.Categoria,
                        pt.NivelDificuldade
                    FROM PhishingTemplateEmpresa pte
                    JOIN PhishingTemplate pt ON pte.IdTemplate = pt.IdTemplate
                    WHERE pte.IdEmpresa = @IdEmpresa";

                var templates = db.Query(templatesSql, new { IdEmpresa = idEmpresa }).ToList();
                var result = new List<object>();

                foreach (var t in templates)
                {
                    var paramsSql = @"
                        SELECT 
                            tp.IdParameter,
                            tp.ParameterName,
                            tp.ExampleValue,
                            pv.ParameterValue
                        FROM TemplateParameter tp
                        LEFT JOIN ParameterValue pv ON tp.IdParameter = pv.IdParameter AND pv.IdTemplateEmpresa = @IdTemplateEmpresa
                        WHERE tp.IdTemplate = @IdTemplate";

                    var parameters = db.Query(paramsSql, new { IdTemplateEmpresa = t.IdTemplateEmpresa, IdTemplate = t.IdTemplate }).ToList();

                    result.Add(new
                    {
                        t.IdTemplateEmpresa,
                        t.IdEmpresa,
                        t.IdTemplate,
                        t.NomePersonalizado,
                        t.NomeTemplate,
                        t.Subject,
                        t.BodyMail,
                        t.Categoria,
                        t.NivelDificuldade,
                        Parameters = parameters
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar templates da empresa", error = ex.Message });
            }
        }

        [HttpPost("empresa/{idEmpresa}/customize")]
        public IActionResult CustomizeTemplate(int idEmpresa, [FromBody] CustomizeRequest request)
        {
            if (request == null)
                return BadRequest("Dados inválidos.");

            try
            {
                using var db = _dbFactory.CreateConnection();

                // 1. Criar novo PhishingTemplateEmpresa
                var insertPteSql = @"
                    INSERT INTO PhishingTemplateEmpresa (IdEmpresa, IdTemplate, NomePersonalizado)
                    VALUES (@IdEmpresa, @IdTemplate, @NomePersonalizado);
                    SELECT LAST_INSERT_ID();";

                var newPteId = db.ExecuteScalar<int>(insertPteSql, new
                {
                    IdEmpresa = idEmpresa,
                    IdTemplate = request.IdTemplate,
                    NomePersonalizado = request.NomePersonalizado
                });

                // 2. Salvar parâmetros customizados
                if (request.Parameters != null)
                {
                    foreach (var p in request.Parameters)
                    {
                        db.Execute(@"
                            INSERT INTO ParameterValue (IdParameter, IdTemplateEmpresa, ParameterValue)
                            VALUES (@IdParameter, @IdTemplateEmpresa, @ParameterValue)",
                            new { IdParameter = p.IdParameter, IdTemplateEmpresa = newPteId, ParameterValue = p.ParameterValue });
                    }
                }

                return Ok(new { IdTemplateEmpresa = newPteId, Message = "Template customizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao customizar template", error = ex.Message });
            }
        }
    }
}

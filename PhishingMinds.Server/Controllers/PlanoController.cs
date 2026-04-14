using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;
using System.Data;
using System.Data.SqlClient; // Adicione se for usar SqlConnection padrão

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanoController : ControllerBase
    {
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
    }
}
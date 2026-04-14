using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanoController : ControllerBase
    {
        private static List<Plano> planos = new List<Plano>();

        // GET: api/plano
        [HttpGet]
        public ActionResult<List<Plano>> Get()
        {
            return Ok(planos);
        }

        // GET: api/plano/{id}
        [HttpGet("{id}")]
        public ActionResult<Plano> GetById(int id)
        {
            var plano = planos.FirstOrDefault(p => p.Id_Plano == id);

            if (plano == null)
                return NotFound("Plano não encontrado");

            return Ok(plano);
        }

        [HttpGet("{id}/plano")]
        public ActionResult GetPlanoDaEmpresa(int id)
        {
            var empresa = empresas.FirstOrDefault(e => e.IdEmpresa == id);

            if (empresa == null)
                return NotFound("Empresa não encontrada");

            var plano = planos.FirstOrDefault(p => p.Id_Plano == empresa.IdPlano);

            if (plano == null)
                return NotFound("Plano não encontrado");

            return Ok(plano);
        }

        // POST: api/plano
        [HttpPost]
        public ActionResult Create(Plano novoPlano)
        {
            novoPlano.Id_Plano = planos.Count > 0 ? planos.Max(p => p.Id_Plano) + 1 : 1;

            planos.Add(novoPlano);

            return Ok(novoPlano);
        }

        // PUT: api/plano/{id}
        [HttpPut("{id}")]
        public ActionResult Update(int id, Plano planoAtualizado)
        {
            var plano = planos.FirstOrDefault(p => p.Id_Plano == id);

            if (plano == null)
                return NotFound("Plano não encontrado");

            plano.Nm_Plano = planoAtualizado.Nm_Plano;
            plano.Desc_Plano = planoAtualizado.Desc_Plano;
            plano.Temp_Plano = planoAtualizado.Temp_Plano;
            plano.Value_Plano = planoAtualizado.Value_Plano;
            plano.MaxUsers = planoAtualizado.MaxUsers;
            plano.MaxCampaigns = planoAtualizado.MaxCampaigns;

            return Ok(plano);
        }

        // DELETE: api/plano/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var plano = planos.FirstOrDefault(p => p.Id_Plano == id);

            if (plano == null)
                return NotFound("Plano não encontrado");

            planos.Remove(plano);

            return Ok("Plano removido");
        }
    }

}
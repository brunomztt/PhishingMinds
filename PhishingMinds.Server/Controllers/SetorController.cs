using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetorController : ControllerBase
    {
        private static List<Setor> setores = new List<Setor>();

        // GET: api/setor
        [HttpGet]
        public ActionResult<List<Setor>> Get()
        {
            return Ok(setores);
        }

        // GET: api/setor/{id}
        [HttpGet("{id}")]
        public ActionResult<Setor> GetById(int id)
        {
            var setor = setores.FirstOrDefault(s => s.IdSetor == id);

            if (setor == null)
                return NotFound("Setor não encontrado");

            return Ok(setor);
        }

        // POST: api/setor
        [HttpPost]
        public ActionResult Create(Setor novoSetor)
        {
            novoSetor.IdSetor = setores.Count > 0 ? setores.Max(s => s.IdSetor) + 1 : 1;

            setores.Add(novoSetor);

            return Ok(novoSetor);
        }

        // PUT: api/setor/{id}
        [HttpPut("{id}")]
        public ActionResult Update(int id, Setor setorAtualizado)
        {
            var setor = setores.FirstOrDefault(s => s.IdSetor == id);

            if (setor == null)
                return NotFound("Setor não encontrado");

            setor.IdEmpresa = setorAtualizado.IdEmpresa;
            setor.Nm_Setor = setorAtualizado.Nm_Setor;
            setor.IdGestor = setorAtualizado.IdGestor;

            return Ok(setor);
        }

        // DELETE: api/setor/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var setor = setores.FirstOrDefault(s => s.IdSetor == id);

            if (setor == null)
                return NotFound("Setor não encontrado");

            setores.Remove(setor);

            return Ok("Setor removido");
        }
    }
}
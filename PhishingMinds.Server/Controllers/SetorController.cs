using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetorController : ControllerBase
    {
        private static List<Setor> setores = new List<Setor>();

        [HttpGet]
        public ActionResult<List<Setor>> Get()
        {
            return Ok(setores);
        }

        [HttpGet("{id}")]
        public ActionResult<Setor> GetById(int id)
        {
            var setor = setores.FirstOrDefault(s => s.IdSetor == id);

            if (setor == null)
                return NotFound("Setor não encontrado");

            return Ok(setor);
        }

        [HttpGet("empresa/{idEmpresa}")]
        public ActionResult<List<Setor>> GetByEmpresa(int idEmpresa)
        {
            var lista = setores.Where(s => s.IdEmpresa == idEmpresa).ToList();
            return Ok(lista);
        }

        [HttpPost]
        public ActionResult Create(Setor novoSetor)
        {
            novoSetor.IdSetor = setores.Count > 0 ? setores.Max(s => s.IdSetor) + 1 : 1;
            setores.Add(novoSetor);

            return Ok(novoSetor);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Setor setorAtualizado)
        {
            var setor = setores.FirstOrDefault(s => s.IdSetor == id);

            if (setor == null)
                return NotFound("Setor não encontrado");

            setor.Nm_Setor = setorAtualizado.Nm_Setor;
            setor.IdEmpresa = setorAtualizado.IdEmpresa;
            setor.IdGestor = setorAtualizado.IdGestor;

            return Ok(setor);
        }

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
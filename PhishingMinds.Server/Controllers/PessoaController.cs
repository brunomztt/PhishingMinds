using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaController : ControllerBase
    {
        private static List<Pessoa> pessoas = new List<Pessoa>();

        [HttpGet]
        public ActionResult<IEnumerable<Pessoa>> Get()
        {
            return Ok(pessoas);
        }

        [HttpGet("{id}")]
        public ActionResult<Pessoa> GetById(int id)
        {
            var pessoa = pessoas.FirstOrDefault(p => p.IdUser == id);
            if (pessoa == null)
                return NotFound(new { message = "Pessoa não encontrada" });

            return Ok(pessoa);
        }

        [HttpGet("empresa/{idEmpresa}")]
        public ActionResult<IEnumerable<Pessoa>> GetByEmpresa(int idEmpresa)
        {
            var lista = pessoas.Where(p => p.IdEmpresa == idEmpresa).ToList();
            return Ok(lista);
        }

        [HttpGet("setor/{idSetor}")]
        public ActionResult<IEnumerable<Pessoa>> GetBySetor(int idSetor)
        {
            var lista = pessoas.Where(p => p.IdSetor == idSetor).ToList();
            return Ok(lista);
        }

        [HttpPost]
        public ActionResult<Pessoa> Create([FromBody] Pessoa novaPessoa)
        {
            if (novaPessoa == null)
                return BadRequest();

            pessoas.Add(novaPessoa);

            return CreatedAtAction(nameof(GetById), new { id = novaPessoa.IdUser }, novaPessoa);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Pessoa pessoaAtualizada)
        {
            var pessoa = pessoas.FirstOrDefault(p => p.IdUser == id);
            if (pessoa == null)
                return NotFound(new { message = "Pessoa não encontrada" });

            pessoa.Nome = pessoaAtualizada.Nome;
            pessoa.Email = pessoaAtualizada.Email;
            pessoa.Senha = pessoaAtualizada.Senha;
            pessoa.Ativo = pessoaAtualizada.Ativo;
            pessoa.Dt_cadastro = pessoaAtualizada.Dt_cadastro;
            pessoa.UltimoLogin = pessoaAtualizada.UltimoLogin;
            pessoa.PhishingScore = pessoaAtualizada.PhishingScore;
            pessoa.IdEmpresa = pessoaAtualizada.IdEmpresa;
            pessoa.IdSetor = pessoaAtualizada.IdSetor;
            pessoa.IdCargo = pessoaAtualizada.IdCargo;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var pessoa = pessoas.FirstOrDefault(p => p.IdUser == id);
            if (pessoa == null)
                return NotFound(new { message = "Pessoa não encontrada" });

            pessoas.Remove(pessoa);

            return NoContent();
        }
    }
}
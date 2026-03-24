using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private static List<Empresa> empresas = new List<Empresa>();

        // GET: api/empresa
        [HttpGet]
        public ActionResult<List<Empresa>> Get()
        {
            return Ok(empresas);
        }

        // GET: api/empresa/id da empresa
        [HttpGet("{id}")]
        public ActionResult<Empresa> GetById(int id)
        {
            var empresa = empresas.FirstOrDefault(e => e.IdPlano == id);

            if (empresa == null)
                return NotFound("Empresa não encontrada");

            return Ok(empresa);
        }

        // POST: api/empresa
        [HttpPost]
        public ActionResult Create(Empresa novaEmpresa)
        {
            empresas.Add(novaEmpresa);
            return Ok("Empresa criada com sucesso");
        }

        // PUT: api/empresa/id da empresa
        [HttpPut("{id}")]
        public ActionResult Update(int id, Empresa empresaAtualizada)
        {
            var empresa = empresas.FirstOrDefault(e => e.IdPlano == id);

            if (empresa == null)
                return NotFound("Empresa não encontrada");

            empresa.Nm_Empresa = empresaAtualizada.Nm_Empresa;
            empresa.Nm_Dono = empresaAtualizada.Nm_Dono;
            empresa.Mail = empresaAtualizada.Mail;
            empresa.CNPJ = empresaAtualizada.CNPJ;
            empresa.Dt_Contratacao = empresaAtualizada.Dt_Contratacao;
            empresa.Dt_FimContrato = empresaAtualizada.Dt_FimContrato;
            empresa.Ativo = empresaAtualizada.Ativo;

            return Ok("Empresa atualizada");
        }

        // DELETE: api/empresa/id da empresa
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var empresa = empresas.FirstOrDefault(e => e.IdPlano == id);

            if (empresa == null)
                return NotFound("Empresa não encontrada");

            empresas.Remove(empresa);
            return Ok("Empresa removida");
        }
    }
}
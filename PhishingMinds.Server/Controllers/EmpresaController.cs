using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private static List<Empresa> empresas = new List<Empresa>(); //implementar o banco dps nessas listas (setor tb)

        [HttpGet]
        public ActionResult<List<Empresa>> Get()
        {
            return Ok(empresas);
        }

        [HttpGet("{id}")]
        public ActionResult<Empresa> GetById(int id)
        {
            var empresa = empresas.FirstOrDefault(e => e.IdEmpresa == id);

            if (empresa == null)
                return NotFound("Empresa não encontrada");

            return Ok(empresa);
        }

        [HttpPost]
        public ActionResult Create(Empresa novaEmpresa)
        {
            novaEmpresa.IdEmpresa = empresas.Count > 0 ? empresas.Max(e => e.IdEmpresa) + 1 : 1;
            novaEmpresa.Dt_Cadastro = DateTime.Now;

            empresas.Add(novaEmpresa);

            return Ok(novaEmpresa);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Empresa empresaAtualizada)
        {
            var empresa = empresas.FirstOrDefault(e => e.IdEmpresa == id);

            if (empresa == null)
                return NotFound("Empresa não encontrada");

            empresa.IdPlano = empresaAtualizada.IdPlano;
            empresa.Nm_Empresa = empresaAtualizada.Nm_Empresa;
            empresa.Nm_Dono = empresaAtualizada.Nm_Dono;
            empresa.Mail = empresaAtualizada.Mail;
            empresa.CNPJ = empresaAtualizada.CNPJ;
            empresa.Dt_Contratacao = empresaAtualizada.Dt_Contratacao;
            empresa.Dt_FimContrato = empresaAtualizada.Dt_FimContrato;
            empresa.Ativo = empresaAtualizada.Ativo;

            return Ok(empresa);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var empresa = empresas.FirstOrDefault(e => e.IdEmpresa == id);

            if (empresa == null)
                return NotFound("Empresa não encontrada");

            empresas.Remove(empresa);
            return Ok("Empresa removida");
        }
    }
}
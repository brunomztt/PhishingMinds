using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Class;

namespace PhishingMinds.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : ControllerBase
    {

        private static List<Cargo> cargos = new List<Cargo>();

        [HttpGet]
        public ActionResult<IEnumerable<Cargo>> Get()
        {
            return Ok(cargos);
        }

        [HttpGet("{id}")]
        public ActionResult<Cargo> GetById(int id)
        {
            var cargo = cargos.FirstOrDefault(c => c.Id_Cargo == id);
            if (cargo == null)
                return NotFound(new { message = "Cargo não encontrado" });

            return Ok(cargo);
        }

        [HttpPost]
        public ActionResult<Cargo> Create([FromBody] Cargo novoCargo)
        {
            if (novoCargo == null)
                return BadRequest();

            cargos.Add(novoCargo);

            return CreatedAtAction(nameof(GetById), new { id = novoCargo.Id_Cargo }, novoCargo);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Cargo cargoAtualizado)
        {
            var cargo = cargos.FirstOrDefault(c => c.Id_Cargo == id);
            if (cargo == null)
                return NotFound(new { message = "Cargo não encontrado" });

            cargo.Nm_Cargo = cargoAtualizado.Nm_Cargo;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var cargo = cargos.FirstOrDefault(c => c.Id_Cargo == id);
            if (cargo == null)
                return NotFound(new { message = "Cargo não encontrado" });

            cargos.Remove(cargo);

            return NoContent();
        }
    }
}
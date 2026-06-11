using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.AnimalValue;
using Microsoft.AspNetCore.Mvc;


namespace Chaos.Api.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AnimalValueConfigController : ControllerBase
        {
            private readonly IAnimalValueConfigService _service;

            public AnimalValueConfigController(IAnimalValueConfigService service)
            {
                _service = service;
            }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET BY ID — este sí devuelve SVGs (se llama al hacer clic)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound($"No se encontró configuración con Id: {id}");
            return Ok(result);
        }

        [HttpGet("images")]
        public async Task<IActionResult> GetAnimalImage()
        {
            var result = await _service.GetAnimalImage();
            return Ok(result);
        }

        // CREATE
        [HttpPost]
            public async Task<IActionResult> Create([FromBody] CreateAnimalValueConfigRequest request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }

            // UPDATE
            [HttpPut("{id:guid}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnimalValueConfigRequest request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.UpdateAsync(id, request);

                if (result == null)
                    return NotFound($"No se encontró configuración con Id: {id}");

                return Ok(result);
            }

            // DELETE
            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound($"No se encontró configuración con Id: {id}");

                return NoContent();
            }



        }
    }



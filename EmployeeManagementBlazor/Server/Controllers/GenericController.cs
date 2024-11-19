using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : ControllerBase
    {
        private readonly IGenericRepositoryInterface<T> _geneRepositoryInterface;

        public GenericController(IGenericRepositoryInterface<T> geneRepositoryInterface)
        {
            _geneRepositoryInterface = geneRepositoryInterface;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await _geneRepositoryInterface.GetAll());

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid request sent");
            return Ok(await _geneRepositoryInterface.DeleteById(id));
        }

        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid request sent");
            return Ok(await _geneRepositoryInterface.GetById(id));
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add(T model)
        {
            if (model is null) return BadRequest("Bad request made");
            return Ok(await _geneRepositoryInterface.Insert(model));
        }

        [HttpPut("update")]
        public async Task<IActionResult> update(T model)
        {
            if (model is null) return BadRequest("Bad request made");
            return Ok(_geneRepositoryInterface.Update(model));
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LibrosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = "v2",
                Message = "Obteniendo lista de libros (versión 2 mejorada)",
                Changes = "Incluye precio, autor y stock"
            });
        }
    }
}

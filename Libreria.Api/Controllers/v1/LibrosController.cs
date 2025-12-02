using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Libreria.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LibrosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = "v1",
                Message = "Obteniendo lista de libros (versión 1)"
            });
        }

        // 🔐 MÉTODO PROTEGIDO CON JWT
        [Authorize]
        [HttpGet("protegido")]
        public IActionResult GetProtegido()
        {
            return Ok(new
            {
                Status = "OK",
                Message = "Acceso permitido con JWT válido",
                Usuario = User.Identity?.Name,
                Fecha = DateTime.Now
            });
        }
    }
}

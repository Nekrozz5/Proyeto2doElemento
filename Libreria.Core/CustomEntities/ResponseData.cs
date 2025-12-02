using System.Net;
using System.Text.Json.Serialization;

namespace Libreria.Core.CustomEntities
{
    public class ResponseData
    {
        // Paginación genérica usando objetos (así como estás manejando ahora)
        public PagedList<object> Pagination { get; set; } = default!;

        // Mensajes de retorno (info, warning, error)
        public Message[] Messages { get; set; } = System.Array.Empty<Message>();

        // StatusCode NO debe serializarse porque ya lo maneja el Controller
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public ResponseData() { }
    }
}

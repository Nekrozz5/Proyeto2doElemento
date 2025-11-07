namespace Libreria.Core.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public ApiError? Error { get; set; }

        // Constructor de éxito
        public ApiResponse(T data, string message = "Operación exitosa.")
        {
            Success = true;
            Message = message;
            Data = data;
        }

        // Constructor de error
        public ApiResponse(string message, ApiError error)
        {
            Success = false;
            Message = message;
            Error = error;
        }
    }

    public class ApiError
    {
        public int StatusCode { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}

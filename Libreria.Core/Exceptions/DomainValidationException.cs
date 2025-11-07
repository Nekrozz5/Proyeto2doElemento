namespace Libreria.Core.Exceptions
{
    public class DomainValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public DomainValidationException(string message, IDictionary<string, string[]> errors = null)
            : base(message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }
}

namespace Application.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }

        public BadRequestException(IEnumerable<string> errors) 
            : base("One or more errors occurred.")
        {
            Errors = errors;
        }

        public IEnumerable<string> Errors { get; } = [];
    }
}

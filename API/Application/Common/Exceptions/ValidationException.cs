using FluentValidation.Results;

namespace Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<ValidationFailure> errors)
        {
            Errors = errors;
        }

        public IEnumerable<ValidationFailure> Errors { get; }
    }
}
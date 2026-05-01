namespace Application.Features.Reviews.Commands.Delete
{
    public sealed record DeleteReviewCommand(Guid Id) : IRequest;
}

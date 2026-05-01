namespace Application.Features.Courses.Queries.GetSuggestions
{
    public record GetCourseSuggestionsQuery(string Term) : IRequest<IEnumerable<string>>;
}

using Application.Common.Interfaces;

namespace Application.Features.Courses.Queries.GetSuggestions
{
    public class GetCourseSuggestionsQueryHandler(ICourseRepository repository) 
        : IRequestHandler<GetCourseSuggestionsQuery, IEnumerable<string>>
    {
        public async Task<IEnumerable<string>> Handle(GetCourseSuggestionsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Term))
                return [];

            return await repository.GetSuggestionsAsync(request.Term, cancellationToken);
        }
    }
}

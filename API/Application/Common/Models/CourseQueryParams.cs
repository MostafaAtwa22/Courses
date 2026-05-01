namespace Application.Common.Models;

public class CourseQueryParams : QueryParams
{
    public string? Category { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
}

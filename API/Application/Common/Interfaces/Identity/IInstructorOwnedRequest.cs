namespace Application.Common.Interfaces.Identity
{
    public interface IInstructorOwnedRequest
    {
        Guid Id { get; }
        ResourceType ResourceType { get; }
    }

    public enum ResourceType
    {
        Course,
        Section,
        Content
    }
}

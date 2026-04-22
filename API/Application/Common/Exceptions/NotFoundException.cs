namespace Application.Common.Exceptions
{
    public class NotFoundException (string entityName, Guid id) : 
        Exception($"Entity \"{entityName}\" with key \"{id}\" was not found.");
}
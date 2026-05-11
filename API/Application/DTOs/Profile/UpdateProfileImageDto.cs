using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Profile
{
    public class UpdateProfileImageDto
    {
        public IFormFile Image { get; set; } = default!;
    }
}

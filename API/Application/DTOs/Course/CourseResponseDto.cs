using Domain.Enums;

namespace Application.DTOs.Course
{
    public class CourseResponseDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.InProgress;
        public int Cost { get; set; }
        public int StudentCount { get; set; }
        public int TotalReviews { get; set; }
        public decimal AverageRate { get; set; }
        public string Category { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorProfilePicture { get; set; } = string.Empty;
        public string InstructorTitle { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string[] WhatYouWillLearn { get; set; } = [];
        public string[] Requirements { get; set; } = [];
        public string IntroVideoUrl { get; set; } = string.Empty;
    }
}
using Application.Behaviors;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Application.DTOs.Content;
using Application.Features.Contents.Queries.GetByCourse;
using Application.Features.Contents.Queries.GetById;
using Application.Features.Contents.Queries.GetBySection;
using FluentAssertions;
using MediatR;
using Moq;

namespace Application.Tests.Behaviors
{
    public class ContentAccessBehaviorTests
    {
        private readonly Mock<IContentAccessService> _contentAccessServiceMock;
        private readonly Mock<RequestHandlerDelegate<object>> _nextMock;
        private readonly ContentAccessBehavior<IRequireContentAccess, object> _behavior;

        public ContentAccessBehaviorTests()
        {
            _contentAccessServiceMock = new Mock<IContentAccessService>();
            _nextMock = new Mock<RequestHandlerDelegate<object>>();
            _behavior = new ContentAccessBehavior<IRequireContentAccess, object>(_contentAccessServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldRedactSingleContentUrl_WhenNoAccess()
        {
            var courseId = Guid.NewGuid();
            var content = new ContentResponseDto { Id = Guid.NewGuid(), ContentUrl = "http://example.com/video.mp4", IsPreview = false };
            var request = new GetContentByIdQuery(Guid.NewGuid(), courseId);
            
            _nextMock.Setup(x => x(CancellationToken.None)).ReturnsAsync(content);
            _contentAccessServiceMock.Setup(x => x.HasFullCourseContentAccessAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            var contentResult = result as ContentResponseDto;
            contentResult.Should().NotBeNull();
            contentResult!.ContentUrl.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldNotRedactSingleContentUrl_WhenHasFullAccess()
        {
            var courseId = Guid.NewGuid();
            var content = new ContentResponseDto { Id = Guid.NewGuid(), ContentUrl = "http://example.com/video.mp4", IsPreview = false };
            var request = new GetContentByIdQuery(Guid.NewGuid(), courseId);
            
            _nextMock.Setup(x => x(CancellationToken.None)).ReturnsAsync(content);
            _contentAccessServiceMock.Setup(x => x.HasFullCourseContentAccessAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            var contentResult = result as ContentResponseDto;
            contentResult.Should().NotBeNull();
            contentResult!.ContentUrl.Should().Be("http://example.com/video.mp4");
        }

        [Fact]
        public async Task Handle_ShouldRedactContentUrlsInList_WhenNoAccess()
        {
            var courseId = Guid.NewGuid();
            var contents = new List<ContentResponseDto>
            {
                new() { Id = Guid.NewGuid(), ContentUrl = "http://example.com/video.mp4", IsPreview = false },
                new() { Id = Guid.NewGuid(), ContentUrl = "http://example.com/preview.mp4", IsPreview = true }
            };
            var request = new GetContentBySectionQuery(Guid.NewGuid(), courseId);
            
            _nextMock.Setup(x => x(CancellationToken.None)).ReturnsAsync(contents);
            _contentAccessServiceMock.Setup(x => x.HasFullCourseContentAccessAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            var contentList = result as IEnumerable<ContentResponseDto>;
            contentList.Should().NotBeNull();
            contentList!.First().ContentUrl.Should().BeEmpty(); // Non-preview redacted
            contentList.Last().ContentUrl.Should().Be("http://example.com/preview.mp4"); // Preview not redacted
        }

        [Fact]
        public async Task Handle_ShouldRedactContentUrlsInPaginatedResult_WhenNoAccess()
        {
            var courseId = Guid.NewGuid();
            var paginatedResult = new PaginatedResult<ContentResponseDto>
            {
                Items =
                [
                    new() { Id = Guid.NewGuid(), ContentUrl = "http://example.com/video.mp4", IsPreview = false },
                    new() { Id = Guid.NewGuid(), ContentUrl = "http://example.com/preview.mp4", IsPreview = true }
                ],
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };
            var request = new GetContentByCourseQuery(courseId, new QueryParams());
            
            _nextMock.Setup(x => x(CancellationToken.None)).ReturnsAsync(paginatedResult);
            _contentAccessServiceMock.Setup(x => x.HasFullCourseContentAccessAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            var resultPaginated = result as PaginatedResult<ContentResponseDto>;
            resultPaginated.Should().NotBeNull();
            resultPaginated!.Items.First().ContentUrl.Should().BeEmpty(); // Non-preview redacted
            resultPaginated.Items.Last().ContentUrl.Should().Be("http://example.com/preview.mp4"); // Preview not redacted
        }

        [Fact]
        public async Task Handle_ShouldNotRedactPreviewContent_WhenNoAccess()
        {
            var courseId = Guid.NewGuid();
            var content = new ContentResponseDto { Id = Guid.NewGuid(), ContentUrl = "http://example.com/preview.mp4", IsPreview = true };
            var request = new GetContentByIdQuery(Guid.NewGuid(), courseId);
            
            _nextMock.Setup(x => x(CancellationToken.None)).ReturnsAsync(content);
            _contentAccessServiceMock.Setup(x => x.HasFullCourseContentAccessAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            var contentResult = result as ContentResponseDto;
            contentResult.Should().NotBeNull();
            contentResult!.ContentUrl.Should().Be("http://example.com/preview.mp4"); // Preview not redacted
        }
    }
}

using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace API.Tests
{
    public class IntegrationTestFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        public Mock<ICurrentUserService> CurrentUserServiceMock { get; } = new();
        public Mock<Application.Common.Interfaces.Identity.IIdentityEmailService> IdentityEmailServiceMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.Testing.json", optional: true, reloadOnChange: true);
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(CurrentUserServiceMock.Object);
                services.AddSingleton(IdentityEmailServiceMock.Object);
            });
        }
    }
}

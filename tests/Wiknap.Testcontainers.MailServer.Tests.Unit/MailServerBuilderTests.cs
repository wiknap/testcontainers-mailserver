using Shouldly;

using Xunit;

namespace Wiknap.Testcontainers.MailServer.Tests.Unit;

public sealed class MailServerBuilderTests
{
    private readonly MailServerBuilder builder = new MailServerBuilder().WithDockerEndpoint("https://localhost");

    [Fact]
    public void When_CreateWithDefaultConstructor_Then_BuildsContainerWithDefaultValues()
    {
        // Act
        var container = builder.Build();

        // Assert
        container.Image.Repository.ShouldBe("mailserver/docker-mailserver");
        container.Image.Tag.ShouldBe("14.0.0");
        container.Hostname.ShouldBe("localhost");
        container.AdminEmail.ShouldBe("admin@example.com");
        container.AdminPassword.ShouldBe("passwd123");
    }

    [Fact]
    public void When_CreateWithConfigConstructorAndOverrideDefaults_Then_BuildsContainerWithNewValues()
    {
        // Arrange
        const string email = "user@example.com";
        const string password = "pass321";
        const string repo = "repo/image";
        const string version = "1.0";
        const string hostname = "hostname.com";

        // Act
        var container = builder
            .WithAdminEmail(email)
            .WithAdminPassword(password)
            .WithImage($"{repo}:{version}")
            .WithHostname(hostname)
            .Build();

        // Assert
        container.Image.Repository.ShouldBe(repo);
        container.Image.Tag.ShouldBe(version);
        //container.Hostname.ShouldBe(hostname);
        container.AdminEmail.ShouldBe(email);
        container.AdminPassword.ShouldBe(password);
    }
}

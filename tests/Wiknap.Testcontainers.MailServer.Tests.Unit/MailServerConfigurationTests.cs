using Docker.DotNet.Models;

using DotNet.Testcontainers.Configurations;

using Shouldly;

using Xunit;

namespace Wiknap.Testcontainers.MailServer.Tests.Unit;

public sealed class MailServerConfigurationTests
{
    [Fact]
    public void When_CreateWithDefaultConstructor_Then_ConfigInitialized()
    {
        // Act
        var config = new MailServerConfiguration();

        // Assert
        config.AdminEmail.ShouldBeNull();
        config.AdminPassword.ShouldBeNull();
    }

    [Fact]
    public void When_CreateWithConfigValues_Then_ConfigInitialized()
    {
        // Arrange
        const string email = "user@example.com";
        const string password = "pass321";

        // Act
        var config = new MailServerConfiguration(email, password);

        // Assert
        config.AdminEmail.ShouldBe(email);
        config.AdminPassword.ShouldBe(password);
    }

    [Fact]
    public void When_CreateWithResourceConfiguration_Then_ConfigInitialized()
    {
        // Act
        var config = new MailServerConfiguration(new ResourceConfiguration<CreateContainerParameters>());

        // Assert
        config.AdminEmail.ShouldBeNull();
        config.AdminPassword.ShouldBeNull();
    }

    [Fact]
    public void When_CreateWithContainerConfiguration_Then_ConfigInitialized()
    {
        // Act
        var config = new MailServerConfiguration(new ContainerConfiguration());

        // Assert
        config.AdminEmail.ShouldBeNull();
        config.AdminPassword.ShouldBeNull();
    }

    [Fact]
    public void When_CreateWithMailServerContainerConfiguration_Then_ConfigInitialized()
    {
        // Arrange
        const string email = "user@example.com";
        const string password = "pass321";
        var newConfig = new MailServerConfiguration(email, password);

        // Act
        var config = new MailServerConfiguration(newConfig);

        // Assert
        config.AdminEmail.ShouldBe(email);
        config.AdminPassword.ShouldBe(password);
    }

    [Fact]
    public void When_CreateWithMailServerContainerConfigurations_Then_ConfigInitialized()
    {
        // Arrange
        const string email = "user@example.com";
        const string password = "pass321";
        var newConfig = new MailServerConfiguration(email, password);

        // Act
        var config = new MailServerConfiguration(new MailServerConfiguration(), newConfig);

        // Assert
        config.AdminEmail.ShouldBe(email);
        config.AdminPassword.ShouldBe(password);
    }
}

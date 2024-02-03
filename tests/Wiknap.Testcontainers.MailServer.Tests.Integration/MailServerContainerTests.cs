using System.Net;
using System.Net.Mail;

using Shouldly;

using Xunit;

namespace Wiknap.Testcontainers.MailServer.Tests.Integration;

public sealed class MailServerContainerTests : IDisposable
{
    private readonly CancellationTokenSource cts = new();

    [Fact]
    public async Task Given_DefaultConfiguration_WhenStartAsync_ShouldStartAndAllowLogin()
    {
        // Arrange
        const string email = MailServerBuilder.DefaultAdminEmail;
        const string password = MailServerBuilder.DefaultAdminPassword;
        await using var container = new MailServerBuilder().Build();
        await container.StartAsync(cts.Token);
        using var smtpClient = new SmtpClient();
        smtpClient.Credentials = new NetworkCredential(email, password);
        smtpClient.Host = MailServerBuilder.Host;
        smtpClient.Port = container.SmtpPort;

        // Act & Assert
        await Should.NotThrowAsync(() => smtpClient.SendMailAsync(email, email, "Subject", "Content"));
    }

    public void Dispose() => cts.Dispose();
}

using DotNet.Testcontainers.Containers;

using Microsoft.Extensions.Logging;

namespace Wiknap.Testcontainers.MailServer;

public sealed class MailServerContainer : DockerContainer
{
    private readonly MailServerConfiguration configuration;

    public MailServerContainer(MailServerConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        this.configuration = configuration;
    }

    public ushort SmtpPort => GetMappedPublicPort(MailServerBuilder.SmtpPort);
    public ushort ImapPort => GetMappedPublicPort(MailServerBuilder.ImapPort);
    public string AdminEmail => configuration.AdminEmail ?? MailServerBuilder.DefaultAdminEmail;
    public string AdminPassword => configuration.AdminPassword ?? MailServerBuilder.DefaultAdminPassword;

    public async Task AddEmailAsync(string email, string password, CancellationToken ct)
    {
        await ExecAsync(MailServerSetupCommands.AddEmail(email, password), ct);
        // Delay added, because without that server was failing. Probably setup of an new email takes some time
        await Task.Delay(TimeSpan.FromSeconds(3), ct);
    }
}

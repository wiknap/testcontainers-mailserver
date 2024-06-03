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
        var now = DateTime.Now;
        // After adding the user, we need to wait for a server to reconfigure.
        // For now, I unfortunately didn't find other way than waiting for logs related to it
        while (true)
        {
            var (stdout, _) = await GetLogsAsync(since: now.AddSeconds(1), until: DateTime.Now, ct: ct);

            if (stdout.Contains(".zoo"))
                return;
        }
    }
}

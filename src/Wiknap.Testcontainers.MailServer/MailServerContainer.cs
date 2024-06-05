using System.Diagnostics;

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
        await ExecAsync(MailServerSetupCommands.AddEmail(email, password), ct).ConfigureAwait(false);
        var since = DateTime.Now.AddSeconds(1);

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        while (stopWatch.Elapsed <= TimeSpan.FromSeconds(5))
        {
            if (await WasConfigurationReloadedAsync(since, ct).ConfigureAwait(false))
                return;
        }
    }

    // After adding the user, we need to wait for a server to reconfigure.
    // For now, I unfortunately didn't find other way than waiting for logs related to it
    private async Task<bool> WasConfigurationReloadedAsync(DateTime since, CancellationToken ct)
    {
        var (stdout, _) = await GetLogsAsync(since: since, until: DateTime.Now, ct: ct).ConfigureAwait(false);
        return stdout.Contains(".zoo");
    }
}

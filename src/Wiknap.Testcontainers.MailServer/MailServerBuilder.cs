using Docker.DotNet.Models;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace Wiknap.Testcontainers.MailServer;

public sealed class
    MailServerBuilder : ContainerBuilder<MailServerBuilder, MailServerContainer, MailServerConfiguration>
{
    private const string MailServerImage = "mailserver/docker-mailserver:13.3.1";
    public const string DefaultAdminEmail = "admin@example.com";
    public const string DefaultAdminPassword = "passwd123";
    public const ushort SmtpPort = 587;
    public const ushort ImapPort = 143;
    public const string Host = "localhost";

    public MailServerBuilder()
        : this(new MailServerConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    private MailServerBuilder(MailServerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    protected override MailServerConfiguration DockerResourceConfiguration { get; }

    public MailServerBuilder WithAdminEmail(string adminEmail)
        => Merge(DockerResourceConfiguration, new MailServerConfiguration(adminEmail: adminEmail));

    public MailServerBuilder WithAdminPassword(string adminPassword)
        => Merge(DockerResourceConfiguration, new MailServerConfiguration(adminPassword: adminPassword));

    public override MailServerContainer Build()
    {
        Validate();

        var mailHogBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1
            ? this
            : WithWaitStrategy(Wait.ForUnixContainer()
                .UntilCommandIsCompleted(MailServerSetupCommands.AddEmail(DockerResourceConfiguration.AdminEmail,
                    DockerResourceConfiguration.AdminPassword))
                .UntilPortIsAvailable(SmtpPort));
        return new MailServerContainer(mailHogBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    protected override MailServerBuilder Init()
    {
        return base.Init()
            .WithImage(MailServerImage)
            .WithHostname($"mail.{Host}")
            .WithPortBinding(25, true)
            .WithPortBinding(ImapPort, true)
            .WithPortBinding(465, true)
            .WithPortBinding(SmtpPort, true)
            .WithPortBinding(993, true)
            .WithAdminEmail(DefaultAdminEmail)
            .WithAdminPassword(DefaultAdminPassword);
    }

    protected override MailServerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        => Merge(DockerResourceConfiguration, new MailServerConfiguration(resourceConfiguration));

    protected override MailServerBuilder Clone(IContainerConfiguration resourceConfiguration)
        => Merge(DockerResourceConfiguration, new MailServerConfiguration(resourceConfiguration));

    protected override MailServerBuilder Merge(MailServerConfiguration oldValue, MailServerConfiguration newValue)
        => new(new MailServerConfiguration(oldValue, newValue));
}

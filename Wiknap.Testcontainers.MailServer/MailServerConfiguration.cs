using Docker.DotNet.Models;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace Wiknap.Testcontainers.MailServer;

public sealed class MailServerConfiguration : ContainerConfiguration
{
    private const string DefaultAdminEmail = "admin@example.com";
    private const string DefaultAdminPassword = "passwd123";

    public MailServerConfiguration()
        : this(DefaultAdminEmail, DefaultAdminPassword)
    {
        // Passes the default values upwards to the constructor that takes adminEmail and adminPassword.
    }

    public MailServerConfiguration(string adminEmail, string adminPassword)
    {
        AdminEmail = adminEmail;
        AdminPassword = adminPassword;
    }

    public MailServerConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        AdminEmail = DefaultAdminEmail;
        AdminPassword = DefaultAdminPassword;
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    public MailServerConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        AdminEmail = DefaultAdminEmail;
        AdminPassword = DefaultAdminPassword;
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    public MailServerConfiguration(MailServerConfiguration resourceConfiguration)
        : this(new MailServerConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    public MailServerConfiguration(MailServerConfiguration oldValue, MailServerConfiguration newValue)
        : base(oldValue, newValue)
    {
        AdminEmail = BuildConfiguration.Combine(oldValue.AdminEmail, newValue.AdminEmail);
        AdminPassword = BuildConfiguration.Combine(oldValue.AdminPassword, newValue.AdminPassword);
    }

    public string AdminEmail { get; }
    public string AdminPassword { get; }
}

using System.Diagnostics;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;

using MimeKit;

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

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            await smtpClient.ConnectAsync(MailServerBuilder.Host, container.SmtpPort);
            await smtpClient.AuthenticateAsync(email, password);
        });
    }

    [Fact]
    public async Task Given_DefaultConfiguration_WhenSendEmail_ShouldBeInInbox()
    {
        // Arrange
        const string senderEmail = MailServerBuilder.DefaultAdminEmail;
        const string senderPassword = MailServerBuilder.DefaultAdminPassword;
        const string receiverEmail = "user@example.com";
        const string receiverPassword = MailServerBuilder.DefaultAdminPassword;
        await using var container = new MailServerBuilder().Build();
        await container.StartAsync(cts.Token);
        await container.AddEmailAsync(receiverEmail, receiverPassword, default);
        await SendEmailAsync(senderEmail, senderPassword, container.SmtpPort, receiverEmail, "Subject", "Content");

        // Act
        var content = await GetEmailContentAsync(receiverEmail, receiverPassword, container.ImapPort, "Subject");

        // Assert
        content.ShouldNotBeNull();
    }

    private static async Task SendEmailAsync(string email, string password, ushort smtpPort, string receiverEmail, string subject, string content)
    {
        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(MailServerBuilder.Host, smtpPort);
        await smtpClient.AuthenticateAsync(email, password);
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(email, email));
        mimeMessage.To.Add(new MailboxAddress(receiverEmail, receiverEmail));
        mimeMessage.Subject = subject;
        mimeMessage.Body = new TextPart("plain")
        {
            Text = content
        };
        await smtpClient.SendAsync(mimeMessage);
    }

    private static async Task<string?> GetEmailContentAsync(string email, string password, ushort imapPort, string subject)
    {
        using var imapClient = new ImapClient();
        await imapClient.ConnectAsync(MailServerBuilder.Host, imapPort);
        await imapClient.AuthenticateAsync(email, password);
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        await imapClient.Inbox.OpenAsync(FolderAccess.ReadOnly);
        while (stopWatch.Elapsed <= TimeSpan.FromSeconds(10))
        {
            var searchResult = await imapClient.Inbox.SearchAsync(SearchQuery.SubjectContains(subject));
            if (searchResult.Count == 0)
                continue;

            var id = searchResult.Last();
            var message = await imapClient.Inbox.GetMessageAsync(id);
            return message.TextBody;
        }

        return null;
    }

    public void Dispose() => cts.Dispose();
}

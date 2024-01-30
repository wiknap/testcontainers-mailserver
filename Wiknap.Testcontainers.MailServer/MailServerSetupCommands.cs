namespace Wiknap.Testcontainers.MailServer;

public static class MailServerSetupCommands
{
    private const string Setup = "setup";
    private const string Email = "email";
    private const string Add = "add";
    private static readonly string[] AddCommand = [Setup, Email, Add];

    public static string[] AddEmail(string email, string password) => [..AddCommand, email, password];
}

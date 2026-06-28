namespace Spotflix.Api.Configuration;

public class SmtpSettings
{
    public const string SectionName = "Smtp";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 25;
    public string FromEmail { get; init; } = "noreply@spotflix.local";
    public string FromName { get; init; } = "Spotflix";
}

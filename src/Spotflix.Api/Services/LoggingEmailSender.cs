namespace Spotflix.Api.Services;

/// <summary>
/// Implementação de desenvolvimento: apenas registra o e-mail no log.
/// Em produção, substitua por um provedor real (SMTP, SendGrid, etc.).
/// </summary>
public class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "==== E-MAIL (dev) ====\nPara: {To}\nAssunto: {Subject}\nCorpo:\n{Body}\n======================",
            to, subject, htmlBody);
        return Task.CompletedTask;
    }
}

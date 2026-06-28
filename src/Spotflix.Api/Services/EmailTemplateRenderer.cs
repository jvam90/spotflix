namespace Spotflix.Api.Services;

public class EmailTemplateRenderer(IWebHostEnvironment env)
{
    public string Render(string templateName, Dictionary<string, string> values)
    {
        var path = Path.Combine(env.ContentRootPath, "EmailTemplates", $"{templateName}.html");
        var html = File.ReadAllText(path);

        foreach (var (key, value) in values)
            html = html.Replace($"{{{{{key}}}}}", value);

        return html;
    }
}

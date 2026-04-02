namespace CRM.Medical.API.Health;

public static class StatusPageTemplate
{
    public static string Render(string status, string environment, DateTime timestamp)
    {
        var statusClass = status == "Healthy" ? "ok" : "degraded";
        return $$"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                  <meta charset="utf-8" />
                  <title>CRM Medical — Status</title>
                  <style>
                    body { font-family: system-ui, sans-serif; max-width: 600px; margin: 60px auto; color: #222; }
                    h1   { font-size: 1.6rem; }
                    .ok       { color: #16a34a; font-weight: bold; }
                    .degraded { color: #dc2626; font-weight: bold; }
                    dl { display: grid; grid-template-columns: max-content auto; gap: 4px 16px; }
                  </style>
                </head>
                <body>
                  <h1>CRM Medical API</h1>
                  <dl>
                    <dt>Status</dt>      <dd class="{{statusClass}}">{{status}}</dd>
                    <dt>Environment</dt> <dd>{{environment}}</dd>
                    <dt>Timestamp</dt>   <dd>{{timestamp:O}}</dd>
                  </dl>
                </body>
                </html>
                """;
    }
}

namespace CRM.Medical.API.Health;

internal static class StatusPageTemplate
{
    internal const string Html = """
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>CRM Medical API</title>
          <style>
            :root {
              --bg: #0d1117;
              --text: #e6edf3;
              --muted: #8b949e;
              --ok-bg: #0d2818;
              --ok-border: #238636;
              --ok-text: #3fb950;
              --bad-bg: #2d1515;
              --bad-border: #da3633;
              --bad-text: #ff7b72;
              --pending-bg: #1c2128;
              --pending-border: #d29922;
              --pending-text: #e3b341;
            }
            body {
              font-family: ui-sans-serif, system-ui, sans-serif;
              margin: 0;
              min-height: 100vh;
              background: var(--bg);
              color: var(--text);
              display: flex;
              align-items: center;
              justify-content: center;
              padding: 1.5rem;
            }
            .card {
              max-width: 36rem;
              width: 100%;
              border-radius: 12px;
              border: 1px solid #30363d;
              padding: 1.5rem 1.75rem;
              box-shadow: 0 8px 24px rgba(0,0,0,.35);
            }
            h1 { font-size: 1.35rem; font-weight: 600; margin: 0 0 0.5rem; }
            .env { color: var(--muted); font-size: 0.9rem; margin-bottom: 1.25rem; }
            .status {
              padding: 0.85rem 1rem;
              border-radius: 8px;
              font-size: 0.95rem;
              line-height: 1.45;
            }
            .status.ok {
              background: var(--ok-bg);
              border: 1px solid var(--ok-border);
              color: var(--ok-text);
            }
            .status.bad {
              background: var(--bad-bg);
              border: 1px solid var(--bad-border);
              color: var(--bad-text);
            }
            .status.pending {
              background: var(--pending-bg);
              border: 1px solid var(--pending-border);
              color: var(--pending-text);
            }
          </style>
        </head>
        <body>
          <div class="card">
            <h1>CRM Medical API</h1>
            <p class="env">Environment: <strong>__ENV__</strong></p>
            <div class="status __STATUS_CLASS__">__STATUS_TEXT__</div>
            <p class="env" style="margin-top:1rem"><a href="/swagger" style="color:var(--ok-text)">Open Swagger UI</a></p>
          </div>
        </body>
        </html>
        """;
}

namespace CorporateCMS.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var requestId = Guid.NewGuid().ToString("N");
            _logger.LogError(ex, "Unhandled exception (RequestId: {RequestId})", requestId);
            if (context.Response.HasStarted)
            {
                throw; // cannot modify response
            }
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "text/html; charset=utf-8";
            var isDev = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
            await context.Response.WriteAsync($"<html><body style='font-family:system-ui;padding:40px;'>" +
                "<h2>Beklenmeyen bir hata oluştu</h2>" +
                (isDev ? $"<pre style='white-space:pre-wrap;background:#222;color:#eee;padding:16px;border-radius:6px;'>{System.Net.WebUtility.HtmlEncode(ex.ToString())}</pre>" : "") +
                $"<p>Takip Kodu: <code>{requestId}</code></p>" +
                "<p><a href='/' style='color:#0d6efd;'>Ana sayfaya dön</a></p>" +
                "</body></html>");
        }
    }
}

public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}

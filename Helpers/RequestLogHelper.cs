namespace PixelMart.API.Helpers;

public class RequestLogHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RequestLogHelper> _logger;

    public RequestLogHelper(IHttpContextAccessor httpContextAccessor, ILogger<RequestLogHelper> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public void LogInfo(string message)
    {
        var context = _httpContextAccessor.HttpContext;
        var username = context?.User?.Identity?.Name ?? "Anonymous";
        var timestamp = DateTime.Now;

        _logger.LogInformation("[{Time}] User: {User} - {Message}", timestamp, username, message);
    }

    public void LogError(Exception ex, string message)
    {
        var context = _httpContextAccessor.HttpContext;
        var username = context?.User?.Identity?.Name ?? "Anonymous";
        var timestamp = DateTime.Now;

        _logger.LogError(ex, "[{Time}] User: {User} - {Message}", timestamp, username, message);
    }

    public Guid GetUserID()
    {
        var context = _httpContextAccessor.HttpContext;
        var userIdClaim = context?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return Guid.Empty;
    }
}

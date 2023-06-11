using System.Net;

public class RestErrorResponse
{
    /// <summary>
    /// Type
    /// </summary>
    public string Type { get; set; } = default!;

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// Status
    /// </summary>
    public HttpStatusCode Status { get; set; }

    /// <summary>
    /// Trace id
    /// </summary>
    public string TraceId { get; set; } = default!;

    /// <summary>
    /// Errors
    /// </summary>
    public Dictionary<string, List<string>> Errors { get; set; } = new();
}
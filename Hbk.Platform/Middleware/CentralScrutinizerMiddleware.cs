using Hbk.Platform.Exceptions;
using Hbk.Platform.Extensions;
using Hbk.Platform.Services;

namespace Hbk.Platform.Middleware;

/// <summary>
/// Hbk.Platform Central Scrutinizer middleware
/// Will keep a log of recent user actions for security purposes.
/// 
/// Author: Mark Brown
/// Authored: 01/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class CentralScrutinizerMiddleware (ICentralScrutinizerService _centralScrutinizer): IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Update service with the user's action
        _centralScrutinizer.RecordAction(context);

        await next(context);
        
        // Format exceptions - use when logging to file
        try
        {
            // await next here
        }
        catch (Exception ex)
        {
            throw new HbkException(LogFormatterExtensions.FormatException(ex));
        }
    }
    
}
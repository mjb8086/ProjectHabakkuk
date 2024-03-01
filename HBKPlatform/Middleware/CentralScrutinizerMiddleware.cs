using HBKPlatform.Services;

namespace HBKPlatform.Middleware;

/// <summary>
/// HBKPlatform Central Scrutinizer middleware
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
        _centralScrutinizer.RecordAction(context);
        await next(context);
    }
    
}
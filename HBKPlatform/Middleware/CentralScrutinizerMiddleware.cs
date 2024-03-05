using HBKPlatform.Exceptions;
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
        // Update service with the user's action
        _centralScrutinizer.RecordAction(context);
        
        await next(context);
        // If any exceptions are thrown while processing the request, log them.
            /*
        catch (HbkException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            throw;
        }
        */
    }
    
}
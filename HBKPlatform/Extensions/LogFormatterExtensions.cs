using System.Diagnostics;

namespace HBKPlatform.Extensions;

public static class LogFormatterExtensions
{
    public static string FormatException(Exception? exception)
    {
        if (exception == null) return string.Empty;

        var stackTrace = new StackTrace(exception, fNeedFileInfo: true);
        var firstFrame = stackTrace.FrameCount > 0 ? stackTrace.GetFrame(0) : null;

        string formattedMessage = $"{exception.Message}";
        if (firstFrame != null)
        {
            formattedMessage += $" at {firstFrame.GetMethod()} in {firstFrame.GetFileName()}:line {firstFrame.GetFileLineNumber()}";
        }
        return formattedMessage;
    }
}
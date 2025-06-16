// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace AIDevGallery.Utils;

/// <summary>
/// Production-ready logging framework for Darbot Dev.
/// Provides structured logging with different severity levels and destinations.
/// </summary>
internal static class ProductionLogger
{
    private const string LOG_FILE_NAME = "darbot-debug.log";
    private const int MAX_LOG_FILE_SIZE = 10 * 1024 * 1024; // 10MB
    private const int MAX_LOG_FILES = 5;
    
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5
    }

    private static LogLevel _minimumLogLevel = LogLevel.Information;
    private static bool _isInitialized = false;
    private static readonly Queue<LogEntry> _logQueue = new();
    private static readonly object _lockObject = new();

    /// <summary>
    /// Initializes the logging system.
    /// </summary>
    public static async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            // Load log level from configuration
            var logLevelString = await ProductionConfigurationManager.GetSettingAsync("logging_level", "Information");
            if (Enum.TryParse<LogLevel>(logLevelString, out var level))
            {
                _minimumLogLevel = level;
            }

            // Rotate logs if needed
            await RotateLogsIfNeededAsync();
            
            _isInitialized = true;
            LogInformation("ProductionLogger initialized successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to initialize ProductionLogger: {ex}");
        }
    }

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    public static void LogTrace(string message, Exception? exception = null)
    {
        LogMessage(LogLevel.Trace, message, exception);
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    public static void LogDebug(string message, Exception? exception = null)
    {
        LogMessage(LogLevel.Debug, message, exception);
    }

    /// <summary>
    /// Logs an information message.
    /// </summary>
    public static void LogInformation(string message, Exception? exception = null)
    {
        LogMessage(LogLevel.Information, message, exception);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    public static void LogWarning(string message, Exception? exception = null)
    {
        LogMessage(LogLevel.Warning, message, exception);
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    public static void LogError(string message, Exception? exception = null)
    {
        LogMessage(LogLevel.Error, message, exception);
    }

    /// <summary>
    /// Logs a critical message.
    /// </summary>
    public static void LogCritical(string message, Exception? exception = null)
    {
        LogMessage(LogLevel.Critical, message, exception);
    }

    /// <summary>
    /// Logs performance metrics.
    /// </summary>
    public static void LogPerformance(string operation, TimeSpan duration, Dictionary<string, object>? metadata = null)
    {
        var performanceData = new
        {
            Operation = operation,
            DurationMs = duration.TotalMilliseconds,
            Timestamp = DateTimeOffset.UtcNow,
            Metadata = metadata ?? new Dictionary<string, object>()
        };

        LogInformation($"PERFORMANCE: {operation} completed in {duration.TotalMilliseconds:F2}ms", null);
        
        // In production, this could also send to telemetry services
        _ = WritePerformanceDataAsync(performanceData);
    }

    /// <summary>
    /// Logs telemetry data.
    /// </summary>
    public static async Task LogTelemetryAsync(string eventName, Dictionary<string, object> properties)
    {
        try
        {
            var telemetryEnabled = await ProductionConfigurationManager.GetSettingAsync("telemetry_enabled", true);
            if (!telemetryEnabled) return;

            var telemetryData = new
            {
                EventName = eventName,
                Properties = properties,
                Timestamp = DateTimeOffset.UtcNow,
                SessionId = GetSessionId(),
                Version = GetAppVersion()
            };

            LogInformation($"TELEMETRY: {eventName}");
            
            // In production, this would send to telemetry services like Application Insights
            await WriteTelemetryDataAsync(telemetryData);
        }
        catch (Exception ex)
        {
            LogError("Failed to log telemetry", ex);
        }
    }

    private static void LogMessage(LogLevel level, string message, Exception? exception)
    {
        if (!_isInitialized)
        {
            _ = Task.Run(InitializeAsync);
        }

        if (level < _minimumLogLevel) return;

        var logEntry = new LogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            Level = level,
            Message = message,
            Exception = exception,
            ThreadId = Environment.CurrentManagedThreadId,
            ProcessId = Environment.ProcessId
        };

        // Output to debug console
        Debug.WriteLine($"[{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}");
        if (exception != null)
        {
            Debug.WriteLine($"Exception: {exception}");
        }

        // Queue for file writing
        lock (_lockObject)
        {
            _logQueue.Enqueue(logEntry);
            
            // Limit queue size to prevent memory issues
            while (_logQueue.Count > 1000)
            {
                _logQueue.Dequeue();
            }
        }

        // Write to file asynchronously
        _ = Task.Run(WriteQueuedLogsAsync);
    }

    private static async Task WriteQueuedLogsAsync()
    {
        var entriesToWrite = new List<LogEntry>();
        
        lock (_lockObject)
        {
            while (_logQueue.Count > 0)
            {
                entriesToWrite.Add(_logQueue.Dequeue());
            }
        }

        if (entriesToWrite.Count == 0) return;

        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var logFile = await localFolder.CreateFileAsync(LOG_FILE_NAME, CreationCollisionOption.OpenIfExists);
            
            var logLines = new List<string>();
            foreach (var entry in entriesToWrite)
            {
                var logLine = JsonSerializer.Serialize(new
                {
                    timestamp = entry.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    level = entry.Level.ToString(),
                    message = entry.Message,
                    exception = entry.Exception?.ToString(),
                    threadId = entry.ThreadId,
                    processId = entry.ProcessId
                });
                logLines.Add(logLine);
            }

            var content = string.Join("\n", logLines) + "\n";
            await FileIO.AppendTextAsync(logFile, content);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to write logs to file: {ex}");
        }
    }

    private static async Task WritePerformanceDataAsync(object performanceData)
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var perfFile = await localFolder.CreateFileAsync("darbot-performance.log", CreationCollisionOption.OpenIfExists);
            
            var json = JsonSerializer.Serialize(performanceData);
            await FileIO.AppendTextAsync(perfFile, json + "\n");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to write performance data: {ex}");
        }
    }

    private static async Task WriteTelemetryDataAsync(object telemetryData)
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var telemetryFile = await localFolder.CreateFileAsync("darbot-telemetry.log", CreationCollisionOption.OpenIfExists);
            
            var json = JsonSerializer.Serialize(telemetryData);
            await FileIO.AppendTextAsync(telemetryFile, json + "\n");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to write telemetry data: {ex}");
        }
    }

    private static async Task RotateLogsIfNeededAsync()
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var logFile = await localFolder.TryGetItemAsync(LOG_FILE_NAME) as StorageFile;
            
            if (logFile != null)
            {
                var properties = await logFile.GetBasicPropertiesAsync();
                if (properties.Size > MAX_LOG_FILE_SIZE)
                {
                    // Rotate logs
                    for (int i = MAX_LOG_FILES - 1; i > 0; i--)
                    {
                        var oldFile = await localFolder.TryGetItemAsync($"{LOG_FILE_NAME}.{i}") as StorageFile;
                        if (oldFile != null)
                        {
                            if (i == MAX_LOG_FILES - 1)
                            {
                                await oldFile.DeleteAsync();
                            }
                            else
                            {
                                await oldFile.RenameAsync($"{LOG_FILE_NAME}.{i + 1}");
                            }
                        }
                    }
                    
                    await logFile.RenameAsync($"{LOG_FILE_NAME}.1");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to rotate logs: {ex}");
        }
    }

    private static string GetSessionId()
    {
        // Generate or retrieve session ID
        return Guid.NewGuid().ToString("N")[..8];
    }

    private static string GetAppVersion()
    {
        // Get app version from package or assembly
        return "0.3.11-alpha";
    }

    private class LogEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public int ThreadId { get; set; }
        public int ProcessId { get; set; }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Windows.Storage;

namespace AIDevGallery.Utils;

/// <summary>
/// Centralized configuration management for Darbot Dev production settings.
/// </summary>
internal static class ProductionConfigurationManager
{
    private const string CONFIG_FILE_NAME = "darbot-config.json";
    private static readonly Dictionary<string, object> _defaultSettings = new()
    {
        // MCP Settings
        ["mcp_enabled"] = true,
        ["mcp_server_url"] = "http://localhost:8000",
        ["mcp_auto_discover"] = true,
        ["mcp_timeout_seconds"] = 5,
        
        // Local Server Settings
        ["local_server_enabled"] = false,
        ["local_server_port"] = 11434,
        ["local_server_auto_start"] = false,
        
        // NLWeb Settings
        ["nlweb_enabled"] = true,
        ["nlweb_temperature"] = 0.3,
        ["nlweb_auto_suggest"] = true,
        
        // Production Settings
        ["telemetry_enabled"] = true,
        ["error_reporting_enabled"] = true,
        ["performance_monitoring_enabled"] = false,
        ["logging_level"] = "Information",
        
        // Security Settings
        ["allow_external_connections"] = false,
        ["require_authentication"] = false,
        ["max_concurrent_requests"] = 10,
        
        // Performance Settings
        ["cache_enabled"] = true,
        ["cache_size_mb"] = 100,
        ["request_timeout_seconds"] = 30
    };

    private static Dictionary<string, object>? _currentSettings;

    /// <summary>
    /// Loads configuration settings from storage.
    /// </summary>
    public static async Task<Dictionary<string, object>> LoadSettingsAsync()
    {
        if (_currentSettings != null)
        {
            return _currentSettings;
        }

        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var configFile = await localFolder.TryGetItemAsync(CONFIG_FILE_NAME) as StorageFile;
            
            if (configFile != null)
            {
                var json = await FileIO.ReadTextAsync(configFile);
                var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                
                if (settings != null)
                {
                    // Merge with defaults to ensure all keys exist
                    _currentSettings = new Dictionary<string, object>(_defaultSettings);
                    foreach (var kvp in settings)
                    {
                        _currentSettings[kvp.Key] = kvp.Value;
                    }
                    return _currentSettings;
                }
            }
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError("Failed to load configuration", ex);
        }

        // Return defaults if loading fails
        _currentSettings = new Dictionary<string, object>(_defaultSettings);
        return _currentSettings;
    }

    /// <summary>
    /// Saves configuration settings to storage.
    /// </summary>
    public static async Task SaveSettingsAsync(Dictionary<string, object> settings)
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var configFile = await localFolder.CreateFileAsync(CONFIG_FILE_NAME, CreationCollisionOption.ReplaceExisting);
            
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            await FileIO.WriteTextAsync(configFile, json);
            _currentSettings = settings;
            
            ProductionLogger.LogInformation("Configuration saved successfully");
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError("Failed to save configuration", ex);
            throw;
        }
    }

    /// <summary>
    /// Gets a configuration value with type safety.
    /// </summary>
    public static async Task<T> GetSettingAsync<T>(string key, T defaultValue = default!)
    {
        try
        {
            var settings = await LoadSettingsAsync();
            
            if (settings.TryGetValue(key, out var value))
            {
                if (value is JsonElement jsonElement)
                {
                    return jsonElement.Deserialize<T>() ?? defaultValue;
                }
                
                if (value is T typedValue)
                {
                    return typedValue;
                }
                
                // Try to convert the value
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
        catch (Exception ex)
        {
            ProductionLogger.LogWarning($"Failed to get setting '{key}', using default", ex);
        }
        
        return defaultValue;
    }

    /// <summary>
    /// Sets a configuration value.
    /// </summary>
    public static async Task SetSettingAsync<T>(string key, T value)
    {
        try
        {
            var settings = await LoadSettingsAsync();
            settings[key] = value!;
            await SaveSettingsAsync(settings);
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError($"Failed to set setting '{key}'", ex);
            throw;
        }
    }

    /// <summary>
    /// Validates current configuration for production readiness.
    /// </summary>
    public static async Task<ProductionConfigValidationResult> ValidateConfigurationAsync()
    {
        var result = new ProductionConfigValidationResult();
        
        try
        {
            var settings = await LoadSettingsAsync();
            
            // Validate MCP settings
            if (await GetSettingAsync<bool>("mcp_enabled"))
            {
                var mcpUrl = await GetSettingAsync<string>("mcp_server_url");
                if (string.IsNullOrEmpty(mcpUrl) || !Uri.TryCreate(mcpUrl, UriKind.Absolute, out _))
                {
                    result.Errors.Add("Invalid MCP server URL");
                }
            }
            
            // Validate local server settings
            if (await GetSettingAsync<bool>("local_server_enabled"))
            {
                var port = await GetSettingAsync<int>("local_server_port");
                if (port < 1024 || port > 65535)
                {
                    result.Errors.Add("Local server port must be between 1024 and 65535");
                }
            }
            
            // Validate performance settings
            var cacheSize = await GetSettingAsync<int>("cache_size_mb");
            if (cacheSize < 10 || cacheSize > 1000)
            {
                result.Warnings.Add("Cache size should be between 10MB and 1000MB for optimal performance");
            }
            
            var maxRequests = await GetSettingAsync<int>("max_concurrent_requests");
            if (maxRequests < 1 || maxRequests > 100)
            {
                result.Warnings.Add("Max concurrent requests should be between 1 and 100");
            }
            
            result.IsValid = result.Errors.Count == 0;
            
            ProductionLogger.LogInformation($"Configuration validation completed. Valid: {result.IsValid}, Errors: {result.Errors.Count}, Warnings: {result.Warnings.Count}");
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Configuration validation failed: {ex.Message}");
            ProductionLogger.LogError("Configuration validation failed", ex);
        }
        
        return result;
    }
    
    /// <summary>
    /// Resets configuration to default values.
    /// </summary>
    public static async Task ResetToDefaultsAsync()
    {
        try
        {
            await SaveSettingsAsync(new Dictionary<string, object>(_defaultSettings));
            ProductionLogger.LogInformation("Configuration reset to defaults");
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError("Failed to reset configuration", ex);
            throw;
        }
    }
}

/// <summary>
/// Result of configuration validation.
/// </summary>
internal class ProductionConfigValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
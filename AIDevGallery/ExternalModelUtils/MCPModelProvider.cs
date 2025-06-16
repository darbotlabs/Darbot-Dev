// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AIDevGallery.Models;
using AIDevGallery.Utils;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AIDevGallery.ExternalModelUtils;

internal class MCPModelProvider : IExternalModelProvider
{
    private IEnumerable<ModelDetails>? _cachedModels;

    public static MCPModelProvider Instance { get; } = new MCPModelProvider();

    public string Name => "MCP Library";

    public HardwareAccelerator ModelHardwareAccelerator => HardwareAccelerator.MCP;

    public List<string> NugetPackageReferences => ["Microsoft.Extensions.AI"];

    public string ProviderDescription => "Models will run via MCP (Model Context Protocol)";

    public string UrlPrefix => "mcp://";

    public string Icon => $"MCP{AppUtils.GetThemeAssetSuffix()}.svg";

    public string Url => "http://localhost:8000/";

    public async Task<IEnumerable<ModelDetails>> GetModelsAsync(bool ignoreCached = false, CancellationToken cancelationToken = default)
    {
        if (ignoreCached)
        {
            _cachedModels = null;
        }

        if (_cachedModels != null && _cachedModels.Any())
        {
            return _cachedModels;
        }

        try
        {
            ProductionLogger.LogInformation("Starting MCP model discovery");
            
            // Discover MCP models - check for local MCP server endpoints
            var models = new List<ModelDetails>();
            
            // Check common MCP endpoints
            var mcpEndpoints = new[]
            {
                await ProductionConfigurationManager.GetSettingAsync("mcp_server_url", "http://localhost:8000"),
                "http://localhost:3000/",
                "http://127.0.0.1:8000/",
                "http://127.0.0.1:3000/"
            };

            var timeout = await ProductionConfigurationManager.GetSettingAsync("mcp_timeout_seconds", 5);
            var mcpEnabled = await ProductionConfigurationManager.GetSettingAsync("mcp_enabled", true);
            
            if (!mcpEnabled)
            {
                ProductionLogger.LogInformation("MCP integration is disabled in configuration");
                return new List<ModelDetails>();
            }

            foreach (var endpoint in mcpEndpoints.Distinct())
            {
                try
                {
                    using var client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                    
                    ProductionLogger.LogDebug($"Checking MCP endpoint: {endpoint}");
                    
                    // Try to discover MCP models endpoint
                    var response = await client.GetAsync($"{endpoint}v1/models", cancelationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync(cancelationToken);
                        ProductionLogger.LogInformation($"Found MCP server at {endpoint}");
                        
                        // Basic model discovery - would need to parse actual MCP response format
                        models.Add(new ModelDetails
                        {
                            Id = $"mcp-{endpoint.GetHashCode():X}",
                            Name = $"MCP Model ({new Uri(endpoint).Port})",
                            Url = $"mcp://{new Uri(endpoint).Authority}/model",
                            Description = $"Model available via MCP at {endpoint}",
                            HardwareAccelerators = [HardwareAccelerator.MCP],
                            Size = 0, // Unknown size for remote models
                            SupportedOnQualcomm = true,
                            Icon = Icon
                        });
                    }
                }
                catch (Exception ex)
                {
                    ProductionLogger.LogDebug($"MCP endpoint {endpoint} not available: {ex.Message}");
                }
            }

            // If no MCP servers found, add a placeholder for configuration
            if (!models.Any())
            {
                ProductionLogger.LogWarning("No MCP servers detected");
                models.Add(new ModelDetails
                {
                    Id = "mcp-placeholder",
                    Name = "Configure MCP Server",
                    Url = "mcp://localhost:8000/configure",
                    Description = "No MCP servers detected. Configure MCP server endpoints in settings.",
                    HardwareAccelerators = [HardwareAccelerator.MCP],
                    Size = 0,
                    SupportedOnQualcomm = true,
                    Icon = Icon
                });
            }

            _cachedModels = models;
            ProductionLogger.LogInformation($"MCP model discovery completed. Found {models.Count} models");
            return _cachedModels;
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError("Failed to discover MCP models", ex);
            return new List<ModelDetails>();
        }
    }

    public IChatClient? GetIChatClient(string url)
    {
        try
        {
            ProductionLogger.LogDebug($"Creating MCP chat client for URL: {url}");
            
            // Parse MCP URL to extract endpoint and model
            if (url.StartsWith("mcp://"))
            {
                var uri = new Uri(url);
                var baseUrl = $"http://{uri.Authority}";
                var modelId = uri.PathAndQuery.TrimStart('/');
                
                ProductionLogger.LogInformation($"Creating MCP chat client: {baseUrl}, model: {modelId}");
                
                // Create a basic HTTP-based chat client for MCP
                return new MCPChatClient(baseUrl, modelId);
            }
            else
            {
                ProductionLogger.LogWarning($"Invalid MCP URL format: {url}");
            }
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError($"Failed to create MCP chat client for URL: {url}", ex);
        }
        
        return null;
    }

    public string? GetDetailsUrl(ModelDetails details)
    {
        return $"https://modelcontextprotocol.io/docs/models/{details.Name}";
    }

    public string? IChatClientImplementationNamespace { get; } = "MCP";

    public string? GetIChatClientString(string url)
    {
        try
        {
            var uri = new Uri(url);
            var baseUrl = $"http://{uri.Authority}";
            var modelId = uri.PathAndQuery.TrimStart('/');
            return $"new MCPChatClient(\"{baseUrl}\", \"{modelId}\")";
        }
        catch
        {
            return $"// Invalid MCP URL format: {url}";
        }
    }
}
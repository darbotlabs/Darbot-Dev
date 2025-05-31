// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AIDevGallery.Models;
using AIDevGallery.Utils;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // TODO: Implement actual MCP model discovery
            // For now, return empty list until MCP integration is configured
            _cachedModels = new List<ModelDetails>();

            return _cachedModels;
        }
        catch
        {
            return [];
        }
    }

    public IChatClient? GetIChatClient(string url)
    {
        // TODO: Implement MCP chat client integration
        // For now, return null until MCP client implementation is available
        return null;
    }

    public string? GetDetailsUrl(ModelDetails details)
    {
        return $"https://modelcontextprotocol.io/docs/models/{details.Name}";
    }

    public string? IChatClientImplementationNamespace { get; } = "MCP";

    public string? GetIChatClientString(string url)
    {
        var modelId = url.Split('/').LastOrDefault();
        return $"// TODO: Implement MCP client instantiation for model: {modelId}";
    }
}
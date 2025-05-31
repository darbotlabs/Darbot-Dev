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

internal class OmniparserModelProvider : IExternalModelProvider
{
    private IEnumerable<ModelDetails>? _cachedModels;

    public static OmniparserModelProvider Instance { get; } = new OmniparserModelProvider();

    public string Name => "Omniparser";

    public HardwareAccelerator ModelHardwareAccelerator => HardwareAccelerator.OMNIPARSER;

    public List<string> NugetPackageReferences => ["Microsoft.Extensions.AI"];

    public string ProviderDescription => "Document parsing and analysis via Omniparser";

    public string UrlPrefix => "omniparser://";

    public string Icon => $"Omniparser{AppUtils.GetThemeAssetSuffix()}.svg";

    public string Url => "http://localhost:8001/";

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
            // TODO: Implement actual Omniparser model discovery
            // For now, return empty list until Omniparser integration is configured
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
        // TODO: Implement Omniparser chat client integration
        // For now, return null until Omniparser client implementation is available
        return null;
    }

    public string? GetDetailsUrl(ModelDetails details)
    {
        return $"https://omniparser.docs.io/models/{details.Name}";
    }

    public string? IChatClientImplementationNamespace { get; } = "Omniparser";

    public string? GetIChatClientString(string url)
    {
        var modelId = url.Split('/').LastOrDefault();
        return $"// TODO: Implement Omniparser client instantiation for model: {modelId}";
    }
}
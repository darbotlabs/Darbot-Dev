// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AIDevGallery.ExternalModelUtils;

/// <summary>
/// A basic HTTP-based chat client for MCP (Model Context Protocol) endpoints.
/// </summary>
internal class MCPChatClient : IChatClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _modelId;

    public MCPChatClient(string baseUrl, string modelId)
    {
        _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));
        _modelId = modelId ?? throw new ArgumentNullException(nameof(modelId));
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromMinutes(5);
        
        Metadata = new ChatClientMetadata("MCP Client", _baseUrl, _modelId);
    }

    public ChatClientMetadata Metadata { get; }

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ProductionLogger.LogDebug($"MCP chat request started for model: {_modelId}");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            var request = CreateMCPRequest(chatMessages, options);
            var requestJson = JsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/v1/chat/completions", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var mcpResponse = JsonSerializer.Deserialize<MCPChatResponse>(responseJson);

            stopwatch.Stop();
            ProductionLogger.LogPerformance("MCP_ChatResponse", stopwatch.Elapsed, new Dictionary<string, object>
            {
                ["ModelId"] = _modelId,
                ["BaseUrl"] = _baseUrl,
                ["MessageCount"] = chatMessages.Count()
            });

            if (mcpResponse?.Choices?.Any() == true)
            {
                var choice = mcpResponse.Choices.First();
                ProductionLogger.LogInformation($"MCP chat response received successfully for model: {_modelId}");
                
                return new ChatResponse([new ChatMessage(ChatRole.Assistant, choice.Message?.Content ?? "No response")])
                {
                    ModelId = _modelId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    FinishReason = choice.FinishReason switch
                    {
                        "stop" => ChatFinishReason.Stop,
                        "length" => ChatFinishReason.Length,
                        _ => ChatFinishReason.Unknown
                    }
                };
            }

            ProductionLogger.LogWarning($"MCP server returned empty response for model: {_modelId}");
            return new ChatResponse([new ChatMessage(ChatRole.Assistant, "No response from MCP server")])
            {
                ModelId = _modelId,
                CreatedAt = DateTimeOffset.UtcNow,
                FinishReason = ChatFinishReason.Unknown
            };
        }
        catch (HttpRequestException ex)
        {
            ProductionLogger.LogError($"MCP HTTP request failed for model {_modelId}", ex);
            return new ChatResponse([new ChatMessage(ChatRole.Assistant, $"MCP Connection Error: {ex.Message}")])
            {
                ModelId = _modelId,
                CreatedAt = DateTimeOffset.UtcNow,
                FinishReason = ChatFinishReason.Unknown
            };
        }
        catch (TaskCanceledException ex)
        {
            ProductionLogger.LogWarning($"MCP request timeout for model {_modelId}", ex);
            return new ChatResponse([new ChatMessage(ChatRole.Assistant, "MCP Request timed out")])
            {
                ModelId = _modelId,
                CreatedAt = DateTimeOffset.UtcNow,
                FinishReason = ChatFinishReason.Unknown
            };
        }
        catch (JsonException ex)
        {
            ProductionLogger.LogError($"MCP response parsing failed for model {_modelId}", ex);
            return new ChatResponse([new ChatMessage(ChatRole.Assistant, "MCP Response parsing error")])
            {
                ModelId = _modelId,
                CreatedAt = DateTimeOffset.UtcNow,
                FinishReason = ChatFinishReason.Unknown
            };
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError($"Unexpected MCP error for model {_modelId}", ex);
            return new ChatResponse([new ChatMessage(ChatRole.Assistant, $"MCP Error: {ex.Message}")])
            {
                ModelId = _modelId,
                CreatedAt = DateTimeOffset.UtcNow,
                FinishReason = ChatFinishReason.Unknown
            };
        }
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ProductionLogger.LogDebug($"MCP streaming chat request started for model: {_modelId}");
        
        try
        {
            // For now, convert non-streaming response to streaming
            var response = await GetResponseAsync(chatMessages, options, cancellationToken);
            
            foreach (var message in response.Messages)
            {
                yield return new ChatResponseUpdate
                {
                    Text = message.Text,
                    Role = message.Role,
                    ModelId = response.ModelId,
                    CreatedAt = response.CreatedAt,
                    FinishReason = response.FinishReason
                };
            }
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError($"MCP streaming error for model {_modelId}", ex);
            yield return new ChatResponseUpdate
            {
                Text = $"Streaming Error: {ex.Message}",
                Role = ChatRole.Assistant,
                ModelId = _modelId,
                CreatedAt = DateTimeOffset.UtcNow,
                FinishReason = ChatFinishReason.Unknown
            };
        }
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return serviceType == typeof(HttpClient) ? _httpClient : null;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    private object CreateMCPRequest(IEnumerable<ChatMessage> messages, ChatOptions? options)
    {
        return new
        {
            model = _modelId,
            messages = messages.Select(m => new
            {
                role = m.Role.ToString().ToLowerInvariant(),
                content = m.Text
            }).ToArray(),
            max_tokens = options?.MaxOutputTokens ?? 2048,
            temperature = options?.Temperature ?? 0.7,
            stream = false
        };
    }

    private class MCPChatResponse
    {
        public MCPChoice[]? Choices { get; set; }
    }

    private class MCPChoice
    {
        public MCPMessage? Message { get; set; }
        public string? FinishReason { get; set; }
    }

    private class MCPMessage
    {
        public string? Content { get; set; }
    }
}
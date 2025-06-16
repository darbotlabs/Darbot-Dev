// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AIDevGallery.Utils;

/// <summary>
/// Natural Language Web (NLWeb) integration for Darbot Dev.
/// Provides natural language interfaces for web-based AI interactions.
/// </summary>
internal static class NLWebIntegration
{
    /// <summary>
    /// Converts natural language queries to structured web API calls.
    /// </summary>
    public static async Task<NLWebResponse> ProcessNaturalLanguageQuery(
        string query, 
        IChatClient? chatClient = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ProductionLogger.LogInformation($"Processing NLWeb query: {query}");
            
            if (string.IsNullOrWhiteSpace(query))
            {
                ProductionLogger.LogWarning("Empty NLWeb query received");
                return new NLWebResponse
                {
                    Success = false,
                    Error = "Query cannot be empty"
                };
            }
            
            if (chatClient == null)
            {
                ProductionLogger.LogWarning("No chat client available for NLWeb processing");
                return new NLWebResponse
                {
                    Success = false,
                    Error = "No chat client available for NLWeb processing"
                };
            }

            // Create a system prompt for web API extraction
            var systemPrompt = """
                You are an AI assistant that converts natural language queries into structured web API calls.
                
                Given a user query, determine:
                1. What type of web operation is needed (search, retrieve, analyze, etc.)
                2. What parameters are required
                3. What format the response should be in
                
                Respond with a JSON structure containing:
                - action: The type of operation
                - parameters: Key-value pairs of parameters
                - expected_format: The desired response format
                
                If the query is not web-related, respond with action: "not_web_related".
                """;

            var messages = new[]
            {
                new ChatMessage(ChatRole.System, systemPrompt),
                new ChatMessage(ChatRole.User, query)
            };

            var response = await chatClient.GetResponseAsync(messages, new ChatOptions
            {
                Temperature = 0.1f,
                MaxOutputTokens = 500
            }, cancellationToken);

            var assistantResponse = response.Messages.LastOrDefault()?.Text ?? "";
            
            ProductionLogger.LogInformation($"NLWeb processing completed successfully for query: {query}");

            return new NLWebResponse
            {
                Success = true,
                OriginalQuery = query,
                ProcessedResponse = assistantResponse,
                Suggestions = GenerateWebSuggestions(query)
            };
        }
        catch (ArgumentException ex)
        {
            ProductionLogger.LogError("Invalid arguments for NLWeb processing", ex);
            return new NLWebResponse
            {
                Success = false,
                Error = $"Invalid query format: {ex.Message}"
            };
        }
        catch (TaskCanceledException ex)
        {
            ProductionLogger.LogWarning("NLWeb processing was cancelled", ex);
            return new NLWebResponse
            {
                Success = false,
                Error = "Processing was cancelled or timed out"
            };
        }
        catch (InvalidOperationException ex)
        {
            ProductionLogger.LogError("Invalid operation during NLWeb processing", ex);
            return new NLWebResponse
            {
                Success = false,
                Error = $"Processing error: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError("Unexpected error during NLWeb processing", ex);
            return new NLWebResponse
            {
                Success = false,
                Error = $"NLWeb processing error: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Generates web-based suggestions for natural language queries.
    /// </summary>
    public static List<string> GenerateWebSuggestions(string query)
    {
        var suggestions = new List<string>();

        // Basic keyword-based suggestions
        var lowerQuery = query.ToLowerInvariant();

        if (lowerQuery.Contains("search") || lowerQuery.Contains("find"))
        {
            suggestions.Add("🔍 Web Search: Use search engines to find information");
            suggestions.Add("📊 Data Search: Query structured databases");
        }

        if (lowerQuery.Contains("api") || lowerQuery.Contains("service"))
        {
            suggestions.Add("🔗 API Call: Make REST API requests");
            suggestions.Add("⚙️ Service Integration: Connect to web services");
        }

        if (lowerQuery.Contains("analyze") || lowerQuery.Contains("process"))
        {
            suggestions.Add("📈 Data Analysis: Analyze web-based data");
            suggestions.Add("🧠 AI Processing: Use AI models for analysis");
        }

        if (lowerQuery.Contains("model") || lowerQuery.Contains("ai"))
        {
            suggestions.Add("🤖 Model Query: Interact with AI models");
            suggestions.Add("🔄 Model Pipeline: Chain multiple AI operations");
        }

        // Default suggestions if none match
        if (suggestions.Count == 0)
        {
            suggestions.Add("💬 Natural Conversation: Ask questions in natural language");
            suggestions.Add("🌐 Web Integration: Connect to web-based AI services");
            suggestions.Add("📱 Cross-Platform: Use via web, mobile, or desktop");
        }

        return suggestions;
    }

    /// <summary>
    /// Converts web responses back to natural language.
    /// </summary>
    public static async Task<string> ConvertResponseToNaturalLanguage(
        string structuredResponse,
        string originalQuery,
        IChatClient? chatClient = null,
        CancellationToken cancellationToken = default)
    {
        if (chatClient == null)
        {
            return $"Response: {structuredResponse}";
        }

        try
        {
            var systemPrompt = """
                You are an AI assistant that converts structured data responses into natural language.
                
                Given a structured response and the original user query, provide a natural, conversational response
                that answers the user's question in an easy-to-understand way.
                
                Make the response:
                - Conversational and friendly
                - Clear and concise
                - Directly addressing the original question
                - Including relevant details from the structured response
                """;

            var messages = new[]
            {
                new ChatMessage(ChatRole.System, systemPrompt),
                new ChatMessage(ChatRole.User, $"Original query: {originalQuery}\n\nStructured response: {structuredResponse}\n\nPlease provide a natural language response:")
            };

            var response = await chatClient.GetResponseAsync(messages, new ChatOptions
            {
                Temperature = 0.3f,
                MaxOutputTokens = 300
            }, cancellationToken);

            return response.Messages.LastOrDefault()?.Text ?? structuredResponse;
        }
        catch
        {
            return $"Based on your query '{originalQuery}', here's what I found: {structuredResponse}";
        }
    }
}

/// <summary>
/// Response structure for NLWeb operations.
/// </summary>
internal class NLWebResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? OriginalQuery { get; set; }
    public string? ProcessedResponse { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}
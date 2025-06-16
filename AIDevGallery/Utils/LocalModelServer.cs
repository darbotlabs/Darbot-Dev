// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AIDevGallery.Utils;

/// <summary>
/// A local HTTP server that exposes AI models for external applications to connect.
/// Similar to Ollama's approach but for local Darbot Dev models.
/// </summary>
internal class LocalModelServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly int _port;
    private bool _isRunning;

    public LocalModelServer(int port = 11434)
    {
        _port = port;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{port}/");
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public bool IsRunning => _isRunning;
    public int Port => _port;

    public async Task StartAsync()
    {
        if (_isRunning) return;

        try
        {
            _listener.Start();
            _isRunning = true;
            
            // Start processing requests in background
            _ = Task.Run(ProcessRequestsAsync, _cancellationTokenSource.Token);
            
            await Task.CompletedTask;
        }
        catch (Exception)
        {
            _isRunning = false;
            throw;
        }
    }

    public void Stop()
    {
        if (!_isRunning) return;

        _cancellationTokenSource.Cancel();
        _listener.Stop();
        _isRunning = false;
    }

    private async Task ProcessRequestsAsync()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested && _listener.IsListening)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleRequestAsync(context), _cancellationTokenSource.Token);
            }
            catch (ObjectDisposedException)
            {
                // Listener was stopped
                break;
            }
            catch (Exception)
            {
                // Log error and continue
                continue;
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            ProductionLogger.LogDebug($"Local server handling request: {request.HttpMethod} {request.Url?.AbsolutePath}");
            
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = 200;
                response.Close();
                return;
            }

            switch (request.Url?.AbsolutePath)
            {
                case "/api/tags":
                case "/v1/models":
                    await HandleModelsRequest(response);
                    break;

                case "/api/generate":
                case "/v1/chat/completions":
                    await HandleChatRequest(request, response);
                    break;

                case "/api/version":
                    await HandleVersionRequest(response);
                    break;

                default:
                    await HandleNotFound(response);
                    break;
            }
            
            ProductionLogger.LogDebug($"Local server request completed: {request.Url?.AbsolutePath}");
        }
        catch (HttpListenerException ex)
        {
            ProductionLogger.LogError($"Local server HTTP listener error for {request.Url?.AbsolutePath}", ex);
            await HandleError(response, "Server connection error");
        }
        catch (InvalidOperationException ex)
        {
            ProductionLogger.LogError($"Local server invalid operation for {request.Url?.AbsolutePath}", ex);
            await HandleError(response, "Invalid request operation");
        }
        catch (Exception ex)
        {
            ProductionLogger.LogError($"Local server unexpected error for {request.Url?.AbsolutePath}", ex);
            await HandleError(response, "Internal server error");
        }
    }

    private async Task HandleModelsRequest(HttpListenerResponse response)
    {
        var models = new
        {
            models = new[]
            {
                new
                {
                    name = "darbot-dev:latest",
                    model = "darbot-dev:latest",
                    size = 1000000000L,
                    digest = "sha256:123456789abcdef",
                    details = new
                    {
                        parent_model = "",
                        format = "gguf",
                        family = "llama",
                        families = new[] { "llama" },
                        parameter_size = "7B",
                        quantization_level = "Q4_K_M"
                    },
                    expires_at = DateTime.UtcNow.AddHours(1)
                }
            }
        };

        await WriteJsonResponse(response, models);
    }

    private async Task HandleChatRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        try
        {
            string requestBody;
            using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Parse the request (basic implementation)
            var chatResponse = new
            {
                model = "darbot-dev:latest",
                created_at = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                message = new
                {
                    role = "assistant",
                    content = "Hello! I'm the Darbot Dev AI assistant. This is a basic response from the local model server. Full integration would require connecting to actual local models."
                },
                done = true,
                total_duration = 1000000000L,
                load_duration = 500000000L,
                prompt_eval_count = 50,
                prompt_eval_duration = 200000000L,
                eval_count = 25,
                eval_duration = 300000000L
            };

            await WriteJsonResponse(response, chatResponse);
        }
        catch (Exception ex)
        {
            await HandleError(response, $"Error processing chat request: {ex.Message}");
        }
    }

    private async Task HandleVersionRequest(HttpListenerResponse response)
    {
        var version = new
        {
            version = "0.3.11-alpha (Darbot Dev)",
            build = "darbot-dev",
            experimental = true
        };

        await WriteJsonResponse(response, version);
    }

    private async Task HandleNotFound(HttpListenerResponse response)
    {
        response.StatusCode = 404;
        var error = new { error = "Not Found" };
        await WriteJsonResponse(response, error);
    }

    private async Task HandleError(HttpListenerResponse response, string message)
    {
        response.StatusCode = 500;
        var error = new { error = message };
        await WriteJsonResponse(response, error);
    }

    private async Task WriteJsonResponse(HttpListenerResponse response, object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        });

        var buffer = Encoding.UTF8.GetBytes(json);
        response.ContentType = "application/json";
        response.ContentLength64 = buffer.Length;
        
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.Close();
    }

    public void Dispose()
    {
        Stop();
        _listener?.Close();
        _cancellationTokenSource?.Dispose();
    }
}
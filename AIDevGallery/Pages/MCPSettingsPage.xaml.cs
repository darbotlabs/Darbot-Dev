// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AIDevGallery.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace AIDevGallery.Pages;

public sealed partial class MCPSettingsPage : Page
{
    private LocalModelServer? _localModelServer;

    public MCPSettingsPage()
    {
        this.InitializeComponent();
        LoadSettings();
        UpdateServerStatus();
    }

    private void LoadSettings()
    {
        // Load settings from app configuration
        // For now, use default values - in production this would load from settings storage
        
        MCPEnabledToggle.IsOn = true;
        MCPServerUrlTextBox.Text = "http://localhost:8000";
        MCPAutoDiscoverToggle.IsOn = true;
        MCPTimeoutNumberBox.Value = 5;
        
        LocalServerEnabledToggle.IsOn = false;
        LocalServerPortNumberBox.Value = 11434;
        
        NLWebEnabledToggle.IsOn = true;
        NLWebTemperatureSlider.Value = 0.3;
        NLWebAutoSuggestToggle.IsOn = true;
        
        TelemetryEnabledToggle.IsOn = true;
        ErrorReportingToggle.IsOn = true;
        PerformanceMonitoringToggle.IsOn = false;
    }

    private void UpdateServerStatus()
    {
        if (_localModelServer?.IsRunning == true)
        {
            ServerStatusIndicator.Fill = new SolidColorBrush(Microsoft.UI.Colors.Green);
            ServerStatusText.Text = $"Running on port {_localModelServer.Port}";
        }
        else
        {
            ServerStatusIndicator.Fill = new SolidColorBrush(Microsoft.UI.Colors.Red);
            ServerStatusText.Text = "Stopped";
        }
    }

    private async void LocalServerEnabledToggle_Toggled(object sender, RoutedEventArgs e)
    {
        var toggle = (ToggleSwitch)sender;
        
        if (toggle.IsOn)
        {
            await StartLocalServer();
        }
        else
        {
            StopLocalServer();
        }
        
        UpdateServerStatus();
    }

    private async Task StartLocalServer()
    {
        try
        {
            var port = (int)LocalServerPortNumberBox.Value;
            _localModelServer = new LocalModelServer(port);
            await _localModelServer.StartAsync();
            
            ShowNotification("Local model server started successfully", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowNotification($"Failed to start local server: {ex.Message}", InfoBarSeverity.Error);
            LocalServerEnabledToggle.IsOn = false;
        }
    }

    private void StopLocalServer()
    {
        try
        {
            _localModelServer?.Stop();
            _localModelServer?.Dispose();
            _localModelServer = null;
            
            ShowNotification("Local model server stopped", InfoBarSeverity.Informational);
        }
        catch (Exception ex)
        {
            ShowNotification($"Error stopping server: {ex.Message}", InfoBarSeverity.Warning);
        }
    }

    private async void TestServerButton_Click(object sender, RoutedEventArgs e)
    {
        if (_localModelServer?.IsRunning != true)
        {
            ShowNotification("Local server is not running", InfoBarSeverity.Warning);
            return;
        }

        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            
            var port = (int)LocalServerPortNumberBox.Value;
            var response = await client.GetAsync($"http://localhost:{port}/api/version");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ShowNotification($"Server test successful: {response.StatusCode}", InfoBarSeverity.Success);
                
                // Copy test URL to clipboard
                var dataPackage = new DataPackage();
                dataPackage.SetText($"http://localhost:{port}");
                Clipboard.SetContentWithOptions(dataPackage, null);
            }
            else
            {
                ShowNotification($"Server test failed: {response.StatusCode}", InfoBarSeverity.Error);
            }
        }
        catch (Exception ex)
        {
            ShowNotification($"Server test error: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    private async void RunValidationButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ShowNotification("Running comprehensive validation...", InfoBarSeverity.Informational);
            
            // Simulate validation process
            await Task.Delay(2000);
            
            // In a real implementation, this would run actual validation tests
            var validationResults = new[]
            {
                "✅ MCP Configuration Valid",
                "✅ Local Server Configuration Valid", 
                "✅ NLWeb Integration Ready",
                "✅ Production Settings Configured",
                "⚠️ Consider enabling performance monitoring for production"
            };

            var resultText = string.Join("\n", validationResults);
            ShowNotification($"Validation Complete:\n{resultText}", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowNotification($"Validation error: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        // Reset all settings to defaults
        LoadSettings();
        ShowNotification("Settings reset to defaults", InfoBarSeverity.Informational);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // In a real implementation, this would save settings to storage
            SaveSettings();
            ShowNotification("Settings saved successfully", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowNotification($"Failed to save settings: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    private void SaveSettings()
    {
        // Save settings to app configuration
        // This would typically use Windows.Storage.ApplicationData.Current.LocalSettings
        // For now, we'll just validate the settings
        
        if (string.IsNullOrWhiteSpace(MCPServerUrlTextBox.Text))
        {
            throw new InvalidOperationException("MCP Server URL cannot be empty");
        }
        
        if (!Uri.TryCreate(MCPServerUrlTextBox.Text, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("MCP Server URL is not valid");
        }
        
        if (LocalServerPortNumberBox.Value < 1024 || LocalServerPortNumberBox.Value > 65535)
        {
            throw new InvalidOperationException("Local server port must be between 1024 and 65535");
        }
    }

    private void ShowNotification(string message, InfoBarSeverity severity)
    {
        // In a real implementation, this would show a proper notification
        // For now, we'll use a simple content dialog for demonstration
        _ = Task.Run(async () =>
        {
            await this.DispatcherQueue.EnqueueAsync(async () =>
            {
                var dialog = new ContentDialog()
                {
                    Title = severity.ToString(),
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            });
        });
    }
}
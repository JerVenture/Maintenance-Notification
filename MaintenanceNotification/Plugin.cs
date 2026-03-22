using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using MaintenanceNotification.Windows;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ManagedFontAtlas;

namespace MaintenanceNotification;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static INotificationManager NotificationManager { get; private set; } = null!;

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("MaintenanceNotification");
    private MaintenanceNotificationWindow notificationWindow;


    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        ChatGui.ChatMessage += OnChatMessage;

        notificationWindow = new MaintenanceNotificationWindow(Framework);
        WindowSystem.AddWindow(notificationWindow);

        PluginInterface.UiBuilder.OpenMainUi += () => notificationWindow.IsOpen = true;

        Log.Information("MaintenanceNotification Was loaded!");
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        ChatGui.ChatMessage -= OnChatMessage;

        PluginInterface.UiBuilder.OpenMainUi -= () => notificationWindow.IsOpen = true;

        notificationWindow.dispose();
    }



    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type == XivChatType.Notice && message.ToString().Contains("Maintenance will be performed", StringComparison.OrdinalIgnoreCase))
        {
            notificationWindow.IsOpen = true;
            notificationWindow.MaintenanceMessage = message.ToString();
        }
    }
}

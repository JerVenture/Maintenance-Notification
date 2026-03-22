using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using MaintenanceNotification.Windows;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

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

        Log.Information("MaintenanceNotification Was loaded!");
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        ChatGui.ChatMessage -= OnChatMessage;
    }

    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type == XivChatType.Notice && message.ToString().Contains("Maintenance will be performed"))
        {
            notificationWindow.IsOpen = true;
        }
    }
}

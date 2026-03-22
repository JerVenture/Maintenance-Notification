using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using System.Numerics;
using Dalamud.Interface.ManagedFontAtlas;

namespace MaintenanceNotification.Windows;

public class MaintenanceNotificationWindow : Window
{

    private IFontAtlas fontAtlas;
    private IFontHandle maintenanceFontHandle;

    public MaintenanceNotificationWindow(IFramework framework) : base("MAINTENANCE NOTIFICATION")
    {
        SizeCondition = ImGuiCond.Always;
        PositionCondition = ImGuiCond.Always;
        var scale = ImGui.GetIO().FontGlobalScale;

        fontAtlas = Plugin.PluginInterface.UiBuilder.CreateFontAtlas(FontAtlasAutoRebuildMode.Async, false);
        maintenanceFontHandle = fontAtlas.NewDelegateFontHandle(e => e.OnPreBuild(
            tk => tk.AddDalamudDefaultFont(32)));
         
    }     

        public override void PreDraw()
        {
            var viewportSize = ImGui.GetMainViewport().Size;
            Size = new Vector2(viewportSize.X * 0.6f, viewportSize.Y * 0.6f);
            Position = ImGui.GetMainViewport().GetCenter() - ((Size ?? new Vector2(800, 500)) / 2);
        }  
    
    public override void Draw()
    {
        using (maintenanceFontHandle.Push())
        {
                    ImGui.TextWrapped(MaintenanceMessage);
        }
    }

    public void dispose()
    {
        maintenanceFontHandle.Dispose();
        fontAtlas.Dispose();
    }

    public string MaintenanceMessage { get; set; } = string.Empty;
}
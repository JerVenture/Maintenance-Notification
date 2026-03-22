using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using System.Numerics;
using Dalamud.Interface.ManagedFontAtlas;
using Lumina.Data.Parsing.Layer;
using Lumina.Data.Parsing.Scd;

namespace MaintenanceNotification.Windows;

public class MaintenanceNotificationWindow : Window
{

    private IFontAtlas fontAtlas;
    private IFontHandle maintenanceFontHandle;
    private bool titleFlash = false;
    private float flashTimer = 0f;
    private float flashElapsed = 0f;

    public MaintenanceNotificationWindow(IFramework framework) : base("MAINTENANCE NOTIFICATION")
    {
        SizeCondition = ImGuiCond.Always;
        PositionCondition = ImGuiCond.Always;
        var scale = ImGui.GetIO().FontGlobalScale;
        OnOpenSfxId = 42;

        fontAtlas = Plugin.PluginInterface.UiBuilder.CreateFontAtlas(FontAtlasAutoRebuildMode.Async, false);
        maintenanceFontHandle = fontAtlas.NewDelegateFontHandle(e => e.OnPreBuild(
            tk => tk.AddDalamudDefaultFont(32)));
         
    }     

        public override void PreDraw()
        {
            var viewportSize = ImGui.GetMainViewport().Size;
            Size = new Vector2(viewportSize.X * 0.6f, viewportSize.Y * 0.6f);
            Position = ImGui.GetMainViewport().GetCenter() - ((Size ?? new Vector2(800, 500)) / 2);

            if (flashElapsed < 10f)
            {
                if (titleFlash) 
                    ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.3f, 0f, 0f, 1f));
                else
                    ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0f, 0f, 0f, 1f));
            }

        }  
    
    public override void Draw()
    {
        using (maintenanceFontHandle.Push())
        {
            ImGui.TextWrapped(MaintenanceMessage);
        }
    }

    public override void PostDraw()
    {
        if (flashElapsed < 10f)
            ImGui.PopStyleColor();
    }

    public override void Update()
    {
        if (flashElapsed >= 10f)
        {
            WindowName = "MAINTENANCE NOTIFICATION";
            return;
        }

        flashElapsed += ImGui.GetIO().DeltaTime;
        flashTimer += ImGui.GetIO().DeltaTime;

        if (flashTimer >= 0.5f)
        {
            flashTimer = 0f;
            titleFlash = !titleFlash;
            WindowName = titleFlash ? "MAINTENANCE NOTIFICATION" : "***MAINTENANCE SOON***";
        }

    }

    public override void OnOpen()
    {
        flashElapsed = 0f;
        flashTimer = 0f;
    }

    public void dispose()
    {
        maintenanceFontHandle.Dispose();
        fontAtlas.Dispose();
    }

    public string MaintenanceMessage { get; set; } = string.Empty;
}
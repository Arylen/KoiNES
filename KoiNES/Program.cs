using ImGuiNET;
using KoiNES.Emulation;
using KoiNES.UI;
using Raylib_cs;
using rlImGui_cs;

namespace KoiNES;

class Program
{
    private readonly IPanel[] _panels =
    [
        new MainMenuPanel(),
        new CpuStatePanel(),
        new MemoryViewPanel(),
        new VMControlPanel(),
        new NesTestLogPanel(),
        new DasmPanel(),
    ];
    
    private NesVM _nesVM = new();
    
    static void Main(string[] args) => new Program().Run();
    
    private void Run()
    {
        Raylib.InitWindow(1280, 720, "KoiNES");
        Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()));
        rlImGui.Setup();
        
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);

            rlImGui.Begin();
            ImGui.DockSpaceOverViewport();

            foreach (var panel in _panels)
                panel.Draw(_nesVM);
                        
            rlImGui.End();
            
            Raylib.EndDrawing();
        }
        
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
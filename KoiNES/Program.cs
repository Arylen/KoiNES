using ImGuiNET;
using KoiNES.Data;
using KoiNES.UI;
using Raylib_cs;
using rlImGui_cs;

namespace KoiNES;

class Program
{
    static void Main(string[] args) => new Program().Run();

    private IUiElement[] _elements =
    [
        new MainMenuBar(),
        new DisplayWindow(),
        new InputWindow(),
    ];
    
    private void Run()
    {
        Raylib.InitWindow(AppState.WindowWidth, AppState.WindowHeight, "KoiNES");
        Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()));
        rlImGui.Setup();
        
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);

            rlImGui.Begin();
            ImGui.DockSpaceOverViewport();
            
            foreach (var e in _elements)
                e.Draw();
            
            rlImGui.End();
            
            Raylib.EndDrawing();
        }
        
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}
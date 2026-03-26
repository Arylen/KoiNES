using KoiNES.Emulation;

namespace KoiNES.Data;

public static class AppState
{
    public static readonly int WindowWidth = 1280;
    public static readonly int WindowHeight = 720;
    public static NesVM NesVM { get; } = new NesVM();
}
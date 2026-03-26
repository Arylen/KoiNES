using System.Numerics;
using ImGuiNET;

namespace KoiNES.UI;

public class DisplayWindow : IUiElement
{
    private const float NesWidth = 256f;
    private const float NesHeight = 240f;
    private const float AspectRatio = NesWidth / NesHeight;

    public void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(NesWidth + 16, NesHeight + 36), ImGuiCond.Once);
        if (ImGui.Begin("NES Display"))
        {
            var avail = ImGui.GetContentRegionAvail();

            float w, h;
            if (avail.X / avail.Y > AspectRatio)
            {
                h = avail.Y;
                w = h * AspectRatio;
            }
            else
            {
                w = avail.X;
                h = w / AspectRatio;
            }

            // Center horizontally
            var offsetX = (avail.X - w) * 0.5f;
            if (offsetX > 0)
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offsetX);

            var pos = ImGui.GetCursorScreenPos();
            ImGui.GetWindowDrawList().AddRectFilled(pos, new Vector2(pos.X + w, pos.Y + h), 0xFFFFFFFF);
            ImGui.Dummy(new Vector2(w, h));
        }

        ImGui.End();
    }
}
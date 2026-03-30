using KoiNES.Emulation;

namespace KoiNES.UI;

public interface IPanel
{
    void Draw(NesVM vm);
}
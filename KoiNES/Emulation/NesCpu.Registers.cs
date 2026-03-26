using KoiNES.Extensions;

namespace KoiNES.Emulation;

public partial class NesCpu
{
    public byte A { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public byte SP { get; set; }
    public ushort PC { get; set; }
    
    public byte P { get; set; }

    public bool GetFlag(Flag flag) => P.GetBit((int)flag);
    public void SetFlag(Flag flag, bool value) => P = P.SetBit((int)flag, value);

    public enum Flag
    {
        C = 0, Carry = C,
        Z = 1, Zero = Z,
        I = 2, InterruptDisable = I,
        D = 3, Decimal = D,
        B = 4, BFlag = B,
        One = 5,
        V = 6, Overflow = V,
        N = 7, Negative = N,
    }
}
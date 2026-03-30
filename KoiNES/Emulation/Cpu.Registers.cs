using KoiNES.Extensions;

namespace KoiNES.Emulation;

public partial class Cpu
{
    public byte A { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public byte SP { get; set; }
    public ushort PC { get; set; }
    
    public byte P { get; set; }
    
    public bool C { get => GetFlag(Flag.C); set => SetFlag(Flag.C, value); }
    public bool Z { get => GetFlag(Flag.Z); set => SetFlag(Flag.Z, value); }
    public bool I { get => GetFlag(Flag.I); set => SetFlag(Flag.I, value); }
    public bool D { get => GetFlag(Flag.D); set => SetFlag(Flag.D, value); }
    public bool B { get => GetFlag(Flag.B); set => SetFlag(Flag.B, value); }
    public bool V { get => GetFlag(Flag.V); set => SetFlag(Flag.V, value); }
    public bool N { get => GetFlag(Flag.N); set => SetFlag(Flag.N, value); }

    public bool GetFlag(Flag flag) => flag == Flag.One || P.GetBit((int)flag);
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
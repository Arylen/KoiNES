namespace KoiNES.Extensions;

public static class BitExtensions
{
    public static bool IsBitSet(this byte value, int bit) => (value & (1 << bit)) != 0;

    public static byte SetBit(this byte value, int bit, bool set) 
        => set ? (byte)(value | (1 << bit)) : (byte)(value & ~(1 << bit));

    public static bool IsBitSet(this ushort value, int bit) => (value & (1 << bit)) != 0;

    public static ushort SetBit(this ushort value, int bit, bool set) 
        => set ? (ushort)(value | (1 << bit)) : (ushort)(value & ~(1 << bit));
}

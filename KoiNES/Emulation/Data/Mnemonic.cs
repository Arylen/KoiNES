namespace KoiNES.Emulation.Data;

public struct Mnemonic(Cpu cpu, byte[] data, string format)
{
    public byte[] Data { get; set; } = data;
    public Cpu Cpu { get; set; } = cpu;
    public string Format { get; set; } = format;

    public override string ToString()
    {
        if (Data.Length != 3)
            Data = Data.Concat(new byte[3 - Data.Length]).ToArray();

        List<(string token, string value)> replacementMap = new()
        {
            ("{byte}", $"{Data[1]:X2}"),
            ("{word}", $"{Data[2]:X2}{Data[1]:X2}"),
            ("{x}", $"{Cpu.X:X2}"),
            ("{y}", $"{Cpu.X:X2}"),
            ("{a}", $"{Cpu.X:X2}"),
        };
        
        var mnemonic = Format;
        
        foreach (var replacement in replacementMap)
            mnemonic = mnemonic.Replace(replacement.token, replacement.value);

        return mnemonic;
    }
}
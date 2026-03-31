namespace KoiNES.Emulation.Data;

public class Cartridge
{
    private const int TrainerBlockSize = 512;
    private const int HeaderSize = 16;
    
    private byte[] Data { get; set; }
    
    private bool HasValidMagicString => Data[0] == 'N' && Data[1] == 'E' && Data[2] == 'S' && Data[3] == 0x1A;

    private int PRGSize => Data[4] * 16384; // Blocks of 16K bytes
    private int CHRSize => Data[5] * 8192; // Blocks of 8K bytes
    private int Mapper => (Data[6] >> 4) | (Data[7] & 0xF0);
    private bool HasTrainer => (Data[6] & 0b100) != 0;

    public byte[] Trainer { get; private set; }
    public byte[] PRGRom { get; private set; }
    public byte[] CHRRom { get; private set; }
    
    public Cartridge(byte[] data)
    {
        Data = data;
        
        if (Data.Length < 16)
            throw new ArgumentException("File seems too small for an iNes ROM.");
        if (!HasValidMagicString)
            throw new ArgumentException("Invalid magic string.");
        if (Mapper != 0)
            throw new ArgumentException($"Mapper {Mapper} is not supported!.");
        
        Trainer = HasTrainer ? Data.Skip(HeaderSize).Take(TrainerBlockSize).ToArray() : [];
        
        var prgRomStart = HeaderSize + (HasTrainer ? TrainerBlockSize : 0);
        PRGRom = Data.Skip(prgRomStart).Take(PRGSize).ToArray();
        
        var chrRomStart = prgRomStart + PRGSize;
        CHRRom = Data.Skip(chrRomStart).Take(CHRSize).ToArray();

        Console.WriteLine($"iNes ROM loaded. (PRGLen={PRGRom.Length}, CHRLen={CHRRom.Length}, Trainer={Trainer.Length})");
    }

    public byte ReadPrgRom(ushort offset)
    {
        // ROM is mirrored from 0xC000 into 0x8000 in this first case for 16KB ROMs.
        if (PRGRom.Length == 16384)
            return PRGRom[offset % 16384];
        return PRGRom[offset];
    }
}
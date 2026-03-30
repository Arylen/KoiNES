namespace KoiNES.Emulation;

public class Bus(NesVM vm)
{
    public NesVM VM { get; set; } = vm;

    private byte[] _iRAM = new byte[0x0800];
    
    /*
     * Memory Map
     * [x] $0000 - $07FF   ( $800 Size)   2KB internal RAM 
     * [x] $0800 - $0FFF   ( $800 Size)   Mirror of $0000 - $07FF 
     * [x] $1000 - $17FF   ( $800 Size)   Mirror of $0000 - $07FF 
     * [x] $1800 - $1FFF   ( $800 Size)   Mirror of $0000 - $07FF
     * [ ] $2000 - $2007   (   $8 Size)   PPU Registers
     * [ ] $2008 - $3FFF   ($1FF8 Size)   Mirrors of $2000-$2007 repeating every 8 bytes
     * [ ] $4000 - $4017   (  $18 Size)   NES APU and IO Registers
     * [ ] $4018 - $401F   (   $8 Size)   APU and IO Functionality (Disabled unless in CPU Test Mode)
     * [ ] $4020 - $FFFF   ($BFE0 Size)   Unmapped, Available for Cartridge Use
     * Typical Cartridge Usage:
     *      [ ] $6000 - $7FFF  ($2000 Size)   Cartridge RAM, when present.
     *      [ ] $8000 - $FFFF  ($8000 Size)   Cartridge ROM and Mapper Registers
     */

    public byte Read(ushort address, bool throwOnError = true)
    {
        // Internal RAM is 0x800 size echoed throughout 0x0000 - 0x1FFF
        if (address <= 0x07FF) return _iRAM[address];
        if (address <= 0x0FFF) return _iRAM[address - 0x0800];
        if (address <= 0x17FF) return _iRAM[address - 0x1000];
        if (address <= 0x1FFF) return _iRAM[address - 0x1800];
        
        if (throwOnError)
            throw new ArgumentOutOfRangeException(nameof(address), $"OOB Read, no mapped device. (Addr=${address:X4})");
        return 0xFF;
    }

    public void Write(ushort address, byte value)
    {
        // Internal RAM is 0x800 size echoed throughout 0x0000 - 0x1FFF
        if (address <= 0x07FF) _iRAM[address] = value;
        else if (address <= 0x0FFF) _iRAM[address - 0x0800] = value;
        else if (address <= 0x17FF) _iRAM[address - 0x1000] = value;
        else if (address <= 0x1FFF) _iRAM[address - 0x1800] = value;
        else throw new ArgumentOutOfRangeException(nameof(address), $"OOB Write, no mapped device. (Addr=${address:X4}, Val={value:X2})");
    }
}
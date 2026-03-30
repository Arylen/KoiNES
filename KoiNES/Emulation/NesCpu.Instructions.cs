namespace KoiNES.Emulation;

public partial class NesCpu
{
    /*
     *  Cycles is the base amount of cycles for this instruction.
     *  Handler returns extra cycle penalty to be added on top of the base cycles, IE to indicate page-crossing.
     */
    public struct Instruction(byte opcode, int cycles, string mnemonic, Func<NesCpu, int> handler)
    {
        public byte Opcode { get; set; } = opcode;
        public int Cycles { get; set; } = cycles;
        public string Mnemonic { get; set; } = mnemonic;
        public Func<NesCpu, int> Handler { get; set; } = handler;
    }

    public static Dictionary<byte, Instruction> Instructions = new();

    static NesCpu()
    {
        for (byte i = 0; i < byte.MaxValue; i++)
            Instructions[i] = new Instruction(i, 2, "== UNIMPL ==", (_) => 0);
        
    }
}
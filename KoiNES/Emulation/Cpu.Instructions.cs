namespace KoiNES.Emulation;

public partial class Cpu
{
    /*
     *  Cycles is the base amount of cycles for this instruction.
     *  Handler returns extra cycle penalty to be added on top of the base cycles, IE to indicate page-crossing.
     */
    public struct Instruction(byte opcode, int length, string mnemonic, Func<Cpu, int> handler, bool isUnimplemented = false)
    {
        public byte Opcode { get; set; } = opcode;
        public string Mnemonic { get; set; } = mnemonic;
        public int Length { get; set; } = length;
        public bool IsUnimplemented { get; set; } = isUnimplemented;
        public Func<Cpu, int> Handler { get; set; } = handler;
    }

    public static Dictionary<byte, Instruction> Instructions = new();

    static Cpu()
    {
        for (var i = 0; i <= byte.MaxValue; i++)
            Instructions[(byte)i] = new Instruction((byte)i, 1, "NOT IMPLEMENTED", (_) => throw new NotImplementedException(), isUnimplemented: true);
        
    }
}
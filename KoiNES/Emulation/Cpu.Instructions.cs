using KoiNES.Extensions;

namespace KoiNES.Emulation;

public partial class Cpu
{
    public struct Instruction(int length, string mnemonic, Func<Cpu, int> handler, bool isUnimplemented = false)
    {
        public string Mnemonic { get; set; } = mnemonic;
        public int Length { get; set; } = length;
        public bool IsUnimplemented { get; set; } = isUnimplemented;
        public Func<Cpu, int> Handler { get; set; } = handler;
    }

    public static Dictionary<byte, Instruction> Instructions = new();

    static Cpu()
    {
        for (var i = 0; i <= byte.MaxValue; i++)
            Instructions[(byte)i] = new Instruction(1, "NOT IMPLEMENTED", (_) => throw new NotImplementedException(), isUnimplemented: true);
        
        // Jumps
        {
            // Absolute
            Instructions[0x4C] = new Instruction(3, "JMP ${word}", cpu =>
            {
                cpu.PC = cpu.FetchNextWord();
                return 3;
            });
            // Indirect
            Instructions[0x6C] = new Instruction(3, "JMP #${word}", cpu =>
            {
                // Indirect JMP has a page-wrapping bug on the actual hardware.
                // Specifically when reading from the address at the immediate 16-bit pointer.
                var pointer = cpu.FetchNextWord();
                
                ushort memoryValue = cpu.Bus.Read(pointer);
                var wrappedPointer = (ushort)((pointer & 0xFF00) | ((pointer + 1) & 0xFF));
                memoryValue |= (ushort)(cpu.Bus.Read(wrappedPointer) << 8);
                
                cpu.PC = memoryValue;
                
                return 5;
            });
        }
        
        // LDX
        {
            // TODO: Revist for addressing mode
            Instructions[0xA2] = new Instruction(2, "LDX #${byte}", cpu =>
            {
                cpu.X = cpu.FetchNext();
                return 2;
            });
        }
        
        // STX
        {
            // Zero Page
            Instructions[0x86] = new Instruction(2, "STX ${byte} = {x}", cpu =>
            {
                cpu.Bus.Write(cpu.FetchNext(), cpu.X);
                return 3;
            });
            
            // Zero Page, Y
            // Instructions[0x96] = new Instruction(2, "STX", cpu => )
        }
        
        // INCREMENTS
        {
            Instructions[0xC8] = new Instruction(1, "INY", cpu => cpu.IncrementRegister(ref cpu.Y));
            Instructions[0xE8] = new Instruction(1, "INX", cpu => cpu.IncrementRegister(ref cpu.X));
        }
    }

    private int IncrementRegister(ref byte register)
    {
        register++;
        Z = register == 0;
        N = register.IsBitSet(7);
        return 2;
    }
}
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
            // Subroutine
            Instructions[0x20] = new Instruction(3, "JSR ${word}", cpu =>
            {
                var jumpTarget = cpu.FetchNextWord();
                cpu.PushStackWord((ushort)(cpu.PC - 1));
                cpu.PC = jumpTarget;
                return 6;
            });
        }
        
        // Branches
        {
            // If Carry
            Instructions[0xB0] = new Instruction(2, "BCS ${sbyte_rel_pc}", cpu =>
            {
                var value = (sbyte)cpu.FetchNext();

                if (!cpu.C) 
                    return 2;
                
                var startPc = cpu.PC;
                cpu.PC = (ushort)(cpu.PC + value);
                
                if ((startPc & 0xFF00) != (cpu.PC & 0xFF00))
                    return 4;
                
                return 3;
            });
        }
        
        // LDX
        {
            // Absolute
            Instructions[0xA2] = new Instruction(2, "LDX #${byte}", cpu =>
            {
                cpu.X = cpu.FetchNext();
                cpu.N = cpu.X.IsBitSet(7);
                cpu.Z = cpu.X == 0;
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
        }
        
        // NOP
        {
            Instructions[0xEA] = new Instruction(1, "NOP", cpu => 2);
        }
        
        // INCREMENTS
        {
            Instructions[0xC8] = new Instruction(1, "INY", cpu => cpu.IncrementRegister(ref cpu.Y));
            Instructions[0xE8] = new Instruction(1, "INX", cpu => cpu.IncrementRegister(ref cpu.X));
        }
        
        // SET FLAGS
        {
            Instructions[0x38] = new Instruction(1, "SEC", cpu => { cpu.C = true; return 2; });
            Instructions[0xF8] = new Instruction(1, "SED", cpu => { cpu.D = true; return 2; });
            Instructions[0x78] = new Instruction(1, "SEI", cpu => { cpu.I = true; return 2; });
        }
        
        // CLEAR FLAGS
        {
            Instructions[0x18] = new Instruction(1, "CLC", cpu => { cpu.C = false; return 2; });
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
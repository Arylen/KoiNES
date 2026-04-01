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
            Instructions[0xB0] = new Instruction(2, "BCS ${sbyte_rel_pc}", cpu => cpu.BranchIfFlagSignedRel(cpu.C));
            // If NOT Carry
            Instructions[0x90] = new Instruction(2, "BCC ${sbyte_rel_pc}", cpu => cpu.BranchIfFlagSignedRel(!cpu.C));
            // If Zero
            Instructions[0xF0] = new Instruction(2, "BEQ ${sbyte_rel_pc}", cpu => cpu.BranchIfFlagSignedRel(cpu.Z));
            // If NOT Zero
            Instructions[0xD0] = new Instruction(2, "BNE ${sbyte_rel_pc}", cpu => cpu.BranchIfFlagSignedRel(!cpu.Z));
            // If Negative
            Instructions[0x30] = new Instruction(2, "BMI ${sbyte_rel_pc}", cpu => cpu.BranchIfFlagSignedRel(cpu.N));
            // If NOT Negative
            Instructions[0x10] = new Instruction(2, "BPL ${sbyte_rel_pc}", cpu => cpu.BranchIfFlagSignedRel(!cpu.N));
            // If Overflow
            Instructions[0x50] = new Instruction(2, "BVS ${sbyte_rel_pc}", cpu => cpu.BranchIfFlagSignedRel(cpu.V));
            // If NOT Overflow
            Instructions[0x70] = new Instruction(2, "BVC ${sbyte_rel_pc}", cpu => cpu.BranchIfFlagSignedRel(!cpu.V));
        }
        
        // Immediate Loads
        {
            Instructions[0xA9] = new Instruction(2, "LDA #${byte}", cpu => cpu.LoadImmediate(ref cpu.A));
            Instructions[0xA2] = new Instruction(2, "LDX #${byte}", cpu => cpu.LoadImmediate(ref cpu.X));
            Instructions[0xA0] = new Instruction(2, "LDY #${byte}", cpu => cpu.LoadImmediate(ref cpu.Y));
        }
        
        // Immediate Stores
        {
            // Zero Page
            Instructions[0x85] = new Instruction(2, "STA ${byte} = {a}", cpu => cpu.StoreImmediateZeroPage(ref cpu.A));
            Instructions[0x86] = new Instruction(2, "STX ${byte} = {x}", cpu => cpu.StoreImmediateZeroPage(ref cpu.X));
            Instructions[0x84] = new Instruction(2, "STY ${byte} = {y}", cpu => cpu.StoreImmediateZeroPage(ref cpu.Y));
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
        
        // Bit Test
        {
            Instructions[0x24] = new Instruction(2, "BIT ${byte} = {a}", cpu => cpu.BitTest(cpu.FetchNext()));
            Instructions[0x2C] = new Instruction(2, "BIT ${word} = {a}", cpu => cpu.BitTest(cpu.FetchNextWord()));
        }
    }

    private int BitTest(ushort address)
    {
        var value = bus.Read(address);

        Z = (A & value) == 0;
        V = value.IsBitSet(6);
        N = value.IsBitSet(7);
        
        return 3;
    }

    private int BranchIfFlagSignedRel(bool flag)
    {
        var value = (sbyte)FetchNext();

        if (!flag) 
            return 2;
    
        var startPc = PC;
        PC = (ushort)(PC + value);
    
        if ((startPc & 0xFF00) != (PC & 0xFF00))
            return 4;
    
        return 3;
    }

    private int IncrementRegister(ref byte register)
    {
        register++;
        Z = register == 0;
        N = register.IsBitSet(7);
        return 2;
    }

    private int LoadImmediate(ref byte register)
    {
        register = FetchNext();
        Z = register == 0;
        N = register.IsBitSet(7);
        return 2;
    }

    private int StoreImmediateZeroPage(ref byte register)
    {
        Bus.Write(FetchNext(), register);
        return 3;
    }
}
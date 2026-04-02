namespace KoiNES.Emulation;

public partial class Cpu(Bus bus)
{
    public int CycleDebt { get; set; }
    public long CycleCount { get; set; } = 7;

    public Bus Bus { get; set; } = bus;

    public byte FetchNext() => Bus.Read(PC++);
    public ushort FetchNextWord() => (ushort)(Bus.Read(PC++) | (Bus.Read(PC++) << 8));

    private int _iFlagDelayStepsLeft = -1;
    private bool _iFlagDelayValue;
    
    public void Cycle()
    {
        CycleCount++;

        if (CycleDebt > 0)
        {
            CycleDebt--;
            return;
        }

        if (_iFlagDelayStepsLeft == 0)
        {
            I = _iFlagDelayValue;
            _iFlagDelayStepsLeft = -1;
        }
        else if (_iFlagDelayStepsLeft > 0)
        {
            _iFlagDelayStepsLeft--;
        }

        var nextOp = FetchNext();
        
        var instruction = Instructions[nextOp];
        if (instruction.IsUnimplemented)
        {
            Console.WriteLine($"Reached unimplemented instruction ${nextOp:X2}");
            PC -= 1;
            CycleCount -= 1;
            return;
        }

        var cycles = instruction.Handler(this);
        
        CycleDebt += cycles - 1;
    }

    public void PushStack(byte value) => Bus.Write((ushort)(0x100 + SP--), value);

    public void PushStackWord(ushort value)
    {
        PushStack((byte)(value >> 8));
        PushStack((byte)(value & 0xFF));
    }

    public byte PopStack() => Bus.Read((ushort)(0x100 + ++SP));
    public ushort PopStackWord() => (ushort)(PopStack() | (PopStack() << 8));
}
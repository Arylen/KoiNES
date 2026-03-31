using System.Text;
using KoiNES.Emulation.Data;

namespace KoiNES.Emulation;

public class NesVM
{
    public Cpu CPU { get; set; }
    public Bus Bus { get; set; }
    
    public Cartridge? Cartridge { get; set; }

    public bool IsPaused { get; set; }
    
    public bool NesTestLogMode { get; set; }
    public event Action<NesTestLog> OnNesTestLog;

    private volatile bool _running;
    private Thread? _workThread;

    public NesVM()
    {
        Reset();
    }

    public void Reset()
    {
        Bus = new Bus(this);
        CPU = new Cpu(Bus)
        {
            SP = 0xFD,
            P = 0x24,
            PC = Bus.ReadWord(0xFFFC)
        };
    }

    public void Start()
    {
        if (_running)
            return;
        _running = true;
        _workThread = new Thread(WorkLoop)
        {
            IsBackground = true,
            Name = "VM Thread"
        };
        _workThread.Start();
    }

    public void Stop()
    {
        _running = false;
        _workThread?.Join();
        _workThread = null;
    }

    private void WorkLoop()
    {
        while (_running)
        {
            Thread.Sleep(16);
            if (IsPaused)
                continue;
            Cycle();
        }
    }

    public void Cycle()
    {
        if (NesTestLogMode && CPU.CycleDebt == 0)
            EmitNesTestLog();
        CPU.Cycle();
        // for (var i = 0; i < 3; i++)
        //     PPU.Cycle();    
    }

    public void Step()
    {
        do
        {
            Cycle();
        } while (CPU.CycleDebt > 0);
    }

    private void EmitNesTestLog()
    {
        var opcode = Bus.Read(CPU.PC);
        
        var instruction = Cpu.Instructions[opcode];
        
        var paddedData = new byte[3]
        {
            opcode,
            Bus.Read((ushort)(CPU.PC + 1)),
            Bus.Read((ushort)(CPU.PC + 2)),
        };
        
        var mnemonic = string.Format(instruction.Mnemonic, paddedData[1], paddedData[2]);
        
        var newLog = new NesTestLog(
            address: CPU.PC,
            bytes: paddedData.Take(instruction.Length).ToArray(),
            mnemonic: mnemonic,
            regA: CPU.A,
            regX: CPU.X,
            regY: CPU.Y,
            regP: CPU.P,
            regSP: CPU.SP,
            ppuX: 0,
            ppuY: 0,
            cycles: CPU.CycleCount
        );
        
        OnNesTestLog?.Invoke(newLog);
    }

    public void LoadROM(byte[] data, bool reset = true)
    {
        try
        {
            IsPaused = true;
            Cartridge = new Cartridge(data);

            if (reset)
                Reset();
            
            if (!NesTestLogMode)
                CPU.PC = Bus.ReadWord(0xFFFC);
            else
                CPU.PC = 0xC000;

            IsPaused = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
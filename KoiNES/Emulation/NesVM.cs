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
    public event Action<string> OnNesTestLog;

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
        var builder = new StringBuilder();
        
        
    }

    public void LoadROM(byte[] data, bool reset = true)
    {
        try
        {
            IsPaused = true;
            Cartridge = new Cartridge(data);

            if (reset)
                Reset();
            
            CPU.PC = Bus.ReadWord(0xFFFC);

            IsPaused = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
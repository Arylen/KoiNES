namespace KoiNES.Emulation;

public class NesVM
{
    public NesCpu CPU { get; set; }
    public Bus Bus { get; set; }

    public bool IsPaused { get; set; }

    private volatile bool _running;
    private Thread? _workThread;

    public NesVM()
    {
        Reset();
    }

    public void Reset()
    {
        CPU = new NesCpu();
        Bus = new Bus(this);
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
        CPU.Cycle();
        // for (var i = 0; i < 3; i++)
        //     PPU.Cycle();    
    }

    public void LoadROM(byte[] data, bool reset = true)
    {
        IsPaused = true;

        if (reset)
            Reset();
        
        // TODO: Read iNES format, load cartridge into bus.
        
        IsPaused = false;
    }
}
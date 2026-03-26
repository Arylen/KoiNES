namespace KoiNES.Emulation;

public class NesVM
{
    public NesCpu NesCpu { get; set; }
    public Bus Bus { get; set; }

    public NesVM()
    {
        Reset();
    }

    public void Reset()
    {
        NesCpu = new NesCpu();
        Bus = new Bus(this);
    }

    public void Cycle()
    {
        NesCpu.Cycle();
    }
}
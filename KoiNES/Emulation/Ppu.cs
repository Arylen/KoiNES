namespace KoiNES.Emulation;

public class Ppu
{
    public int Dot { get; set; }
    public int Line { get; set; }

    public Ppu()
    {
        Reset();
    }
    
    public void Reset()
    {
        Dot = 21;
    }

    public void Cycle()
    {
        Dot++;
        if (Dot > 340)
        {
            Dot = 0;
            Line++;
            if (Line > 261)
            {
                Line = 0;
            }
        }
    }
}
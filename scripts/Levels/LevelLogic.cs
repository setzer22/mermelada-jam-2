using System;

public static class LevelLogic
{
    public static int GetAnnoyanceLevel(int level, Grabbable item, Surface surface)
    {
        int annoyance;

        switch (level)
        {
            case 0: // posh
                annoyance = 2;
                break;
            case 1: // artist
                annoyance = 1;
                break;
            case 2: // blue collar
                annoyance = 1;
                break;
            case 3: // inversor
                annoyance = 1;
                break;
            case 4: // ghost hunter
                annoyance = 0;
                break;
            default: throw new Exception("Uh oh, I didn't think on that...");
        }

        return annoyance;
    }

    public static int GetAnnoyanceLevel(int level, Grabbable item, Switchable switchable)
    {
        int annoyance;

        switch (level)
        {
            case 0: // posh
                if (switchable.Name == "router") annoyance = 9001;
                else annoyance = 1;
                break;
            case 1: // artist
                annoyance = 1;
                break;
            case 2: // blue collar
                annoyance = 1;
                break;
            case 3: // inversor
                annoyance = 1;
                break;
            case 4: // ghost hunter
                annoyance = 0;
                break;
            default: throw new Exception("Uh oh, I didn't think on that...");
        }

        return annoyance;
    }

    public static int GetDayPassedAnnoyanceLevel(int level)
    {
        int annoyance;

        switch (level)
        {
            case 4: // ghost hunter
                annoyance = 30;
                break;
            default:
                annoyance = 0;
                break;
        }

        return annoyance;
    }

    public static int GetMaxAnnoyanceLevel(int level)
    {
        return level switch
        {
            0 => 5, // posh
            1 => 10, // artist
            2 => 15, // blue collar
            3 => 20, // inversor
            4 => 100, // ghost hunter
            _ => int.MaxValue,
        };
    }

    public static int GetMaxInkLevel(int level)
    {
        return level switch
        {
            0 => 6, // posh
            1 => 8, // artist
            2 => 10, // blue collar
            3 => 12, // inversor
            4 => 14, // ghost hunter
            _ => int.MaxValue,
        };
    }
}
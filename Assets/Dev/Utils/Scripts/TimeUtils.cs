using System;
using System.Text;

public class TimeUtils
{
    private static int[] values = new int[5];
    private static char[] units = new char[5] { 'd', 'h', 'm', 's', '\'' };

    public static float TimeScale = 1f;

    /// <summary>
    /// Keep largest units only. Eg: 62 sec -> 1m02, 1.5sec -> 1s50' (2 units)
    /// </summary>
    public static string FormatTime(float sec, int maxUnitCount = 2)
    {
        if (sec == float.PositiveInfinity)
        {
            return "\u221e";
        }
        if (maxUnitCount == 0)
        {
            return "";
        }

        if (maxUnitCount > 5)
        {
            maxUnitCount = 5;
        }

        int intSec = (int)sec;

        int day = intSec / 86400;
        values[0] = day;
        int remainSec = intSec - day * 86400;

        int hour = remainSec / 3600;
        values[1] = hour;
        remainSec -= hour * 3600;

        int min = remainSec / 60;
        values[2] = min;
        remainSec -= min * 60;

        values[3] = remainSec;

        int dSec = (int)(sec * 10f) % 10;
        values[4] = dSec;

        int needUnitCount = maxUnitCount;
        StringBuilder sb = new StringBuilder();

        int firstUnitIdx = int.MaxValue;
        for (int i = 0; i < 5; i++)
        {
            if (needUnitCount == 0)
            {
                break;
            }

            if (values[i] != 0 || needUnitCount >= 5 - i || i > firstUnitIdx)
            {
                if (firstUnitIdx == int.MaxValue)
                {
                    firstUnitIdx = i;
                }
                sb.Append(values[i]).Append(units[i]);
                needUnitCount--;
            }
        }

        return sb.ToString();
    }

    public static float HowManySecFrom(DateTime from)
    {
        double sec =  (DateTime.Now - from).TotalSeconds;
        return (float)sec;
    }
}

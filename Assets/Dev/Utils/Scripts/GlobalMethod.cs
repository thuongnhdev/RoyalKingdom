using System.Collections.Generic;
using UnityEngine;

public static class GlobalMethod
{

    public static string GetDigitToAlph(ulong input, bool isUpper = false)
    {
        if (input == 0)
        {
            return "0";
        }
        string[] unit;
        if (isUpper)
        {
            unit = new string[] { "K", "M", "G", "T", "P", "F", "G", "H", "I", "J", "K", "L", "M", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }
        else
        {
            unit = new string[] { "k", "m", "g", "t", "p", "f", "g", "h", "i", "j", "k", "l", "m", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        }

        int[] cVal = new int[27];

        int index = 0;
        ulong remainder = 0;

        ulong debugOriInput = input;

        ulong tempInput = input;

        while (true)
        {
            if (input >= 1000)
            {
                remainder = input % 1000;
                input = input / 1000;
                index++;
            }
            else
            {
                break;
            }
        }
        if (index == 1)
        {
            input = (input * 1000) + remainder;
            index = 0;
        }

        if (index > 0)
        {
            //UnityEngine.Debug.Log("GetDigitToAlph index > 0[" + debugOriInput + "][" + input + "][.][" + (remainder/1000f) + "][" + unit[index] + "]");
            return string.Format("{0:#.##}{1}", (input + (remainder / 1000f)), unit[index]);
        }
        //else
        //{
        //    UnityEngine.Debug.Log("GetDigitToAlph index >! 0[" + debugOriInput + "][" + input + "] [" + remainder + "][" + unit[index] + "]");
        //}

        return input.ToString();
    }

    public static string GetDigitToAlphabetic(ulong input, bool isUpper = false)
    {
        //string ret = "";
        string value = input.ToString();
        string[] unit;
        if (isUpper)
        {
            unit = new string[] { "", "K", "M", "G", "T", "P", "F", "G", "H", "I", "J", "K", "L", "M", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }
        else
        {
            unit = new string[] { "", "k", "m", "g", "t", "p", "f", "g", "h", "i", "j", "k", "l", "m", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        }

        int[] cVal = new int[27];

        int index = 0;
        ////-값 예외 처리...        
        if (value.Equals("0") || value.Contains("-"))
        {
            return "0";
        }
        while (true)
        {
            string last4 = "";
            if (value.Length >= 4)
            {
                last4 = value.Substring(value.Length - 4);
                int intLast4 = int.Parse(last4);

                cVal[index] = intLast4 % 1000;

                value = value.Remove(value.Length - 3);
            }
            else
            {
                cVal[index] = int.Parse(value);
                break;
            }

            index++;
        }

        if (index > 0)
        {
            int r = cVal[index] * 1000 + cVal[index - 1];
            return string.Format("{0:#.#}{1}", (float)r / 1000f, unit[index]);
        }

        return value;
    }

    public static string GetDigitToAlphabetic(string value, bool isUpper = false)
    {
        string[] unit;
        if (isUpper)
        {
            unit = new string[] { "", "K", "M", "G", "T", "P", "F", "G", "H", "I", "J", "K", "L", "M", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }
        else
        {
            unit = new string[] { "", "k", "m", "g", "t", "p", "f", "g", "h", "i", "j", "k", "l", "m", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        }

        int[] cVal = new int[27];

        int index = 0;

        if (value.Equals("0") || value.Contains("-"))
        {
            return "0";

        }
        while (true)
        {
            string last4 = "";
            if (value.Length >= 4)
            {
                last4 = value.Substring(value.Length - 4);
                int intLast4 = int.Parse(last4);

                cVal[index] = intLast4 % 1000;

                value = value.Remove(value.Length - 3);
            }
            else
            {
                cVal[index] = int.Parse(value);
                break;
            }

            index++;
        }

        if (index > 0)
        {
            int r = cVal[index] * 1000 + cVal[index - 1];
            return string.Format("{0:#.#}{1}", (float)r / 1000f, unit[index]);
        }

        return value;
    }

    public static string GetHourIntToString(int time)
    {
        return (time / 3600).ToString("D2") + ":" + ((time % 3600) / 60).ToString("D2") + ":" + ((time % 3600) % 60).ToString("D2");
    }

    public static string GetMinIntToString(int time)
    {
        return (time / 60).ToString("D2") + ":" + (time % 60).ToString("D2");
    }

    public static string GetThousandCommaText(long data)
    {
        if (data > 0)
        {
            return string.Format("{0:#,###}", data);
        }
        else
        {
            return "0";
        }

    }

    public static bool GetIntToBool(int input)
    {
        if (ConstGlobal._TRUE == input)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int GetBoolToInt(bool input)
    {
        if (input)
        {
            return ConstGlobal._TRUE;
        }
        else
        {
            return ConstGlobal._FALSE;
        }
    }

    public static float GetFlipX(float flipX, bool isRight)
    {
        if (isRight)
        {
            if (flipX > 0)
            {
                return -1 * flipX;
            }
            else
            {
                return flipX;
            }
        }
        else
        {
            if (flipX > 0)
            {
                return flipX;
            }
            else
            {
                return -1 * flipX;
            }
        }
    }

    public static int[] GetRandomInt(int count, int min, int max)
    {
        List<int> randList = new List<int>();
        int[] retArray = new int[count];

        for (int i = 0; i < max - min + 1; ++i)
            randList.Add(i);

        for (int i = 0; i < count; ++i)
        {
            int index = Random.Range(min, max - i);
            retArray[i] = randList[index];
            randList.RemoveAt(index);
        }

        return retArray;
    }

    private static void SplitStringToList(string inputString, ref List<int> retList)
    {
        retList.Clear();

        string[] ableClass = inputString.Split('-'); ;

        for (int i = 0; i < ableClass.Length; i++)
        {
            int tempClass = -1;
            if (int.TryParse(ableClass[i], out tempClass))
            {
                retList.Add(tempClass);
            }
        }
    }

    public static float GetAngleXZ_Rad(Vector3 target1, Vector3 target2)
    {
        Vector3 v = target2 - target1;

        return Mathf.Atan2(v.z, v.x);
    }
    public static float GetAngleXZ(Vector3 target1, Vector3 target2)
    {
        Vector3 v = target2 - target1;

        return (Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg + 180);

        //float radianAng = Mathf.Atan2(heightAng, widthAng);
        //float angle = radianAng * 180 / Mathf.PI;        
    }
    public static void GetPosByAngleDistance(Vector3 target, float angle, float distance, ref Vector3 retPos)
    {
        retPos.x = target.x + distance * Mathf.Cos(angle);
        retPos.y = target.y;
        retPos.z = target.z + distance * Mathf.Sin(angle);
    }
    public static void ChangeLayersRecursively(Transform trans, int layerIndex)
    {
        trans.gameObject.layer = layerIndex;
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, layerIndex);
        }
    }

    public static byte[] GetBody(byte[] origin)
    {
        byte[] body = new byte[origin.Length - 1];
        System.Array.Copy(origin, 1, body, 0, body.Length);
        return body;
    }

    public static string GetSessionStr(int size)
    {
        if (size > 0)
        {
            char[] str = new char[size];
            for (int i = 0; i < str.Length; i++)
            {
                int div = (int)Mathf.Floor(Random.Range(0f, 1f) * 2);
                if (div == 0)
                {      // 0이면 숫자로
                    str[i] = (char)(Random.Range(0f, 1f) * 10 + '0');
                }
                else
                {             // 1이면 알파벳
                    str[i] = (char)(Random.Range(0f, 1f) * 26 + 'A');
                }
            }
            return string.Concat(str) + System.DateTime.Now.Millisecond;
        }
        return "";
    }
    public static string GetSessionForLog(int size, string strPre)
    {
        if (size > 0)
        {
            char[] str = new char[size];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = (char)(Random.Range(0f, 1f) * 26 + 'A');
            }
            return strPre + string.Concat(str) + UnixTimeNow();
        }
        return "";
    }
    public static long UnixTimeNow()
    {
        var timeSpan = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0));
        return (long)timeSpan.TotalSeconds;
    }
}

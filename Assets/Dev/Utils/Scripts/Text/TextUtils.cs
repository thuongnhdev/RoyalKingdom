using System;
using TMPro;

public class TextUtils
{
    private const string FONT_WEIGHT_FORMAT = "<font-weight=\"{0}\">{1}</font-weight>";

    public static string SetFontWeight(int fontWeight, string input)
    {
        return string.Format(FONT_WEIGHT_FORMAT, fontWeight, input);
    }

    public static string RichTextHeavy(string input)
    {
        return SetFontWeight(800, input);
    }

    public static string RichTextBlack(string input)
    {
        return SetFontWeight(900, input);
    }

    public static string FormatNumber(int input)
    {
        if (input < 1e3)
        {
            return input.ToString();
        }

        float rounded;
        if (input < 1e6)
        {
            rounded = input / 1e3f;
            rounded = MathF.Truncate(rounded * 10) / 10;
            return $"{rounded}K";
        }

        if (input < 1e9)
        {
            rounded = input / 1e6f;
            rounded = MathF.Truncate(rounded * 10) / 10;
            return $"{rounded}M";
        }

        rounded = input / 1e9f;
        rounded = MathF.Truncate(rounded * 10) / 10;
        return $"{rounded}B";
    }

    public static string FromRatioToPercent(float ratio)
    {
        float percent = ratio * 100f;
        return $"{percent:#.##}%";
    }
}

public static class TMP_Extension
{
    public static void ToHeavy(this TMP_Text tmp)
    {
        tmp.text = TextUtils.RichTextHeavy(tmp.text);
    }

    public static void ToBlack(this TMP_Text tmp)
    {
        tmp.text = TextUtils.RichTextBlack(tmp.text);
    }
}

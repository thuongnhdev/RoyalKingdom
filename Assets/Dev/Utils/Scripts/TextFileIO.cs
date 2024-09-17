using System.IO;
using System.Text;
using UnityEngine;

public class TextFileIO
{
    public static void WriteText(string filePath, string content, bool relativePath = true, bool append = false)
    {
        var sb = new StringBuilder();
        if (relativePath)
        {
            sb.Append(Application.persistentDataPath).Append("/");
        }
        sb.Append(filePath);

        if (!File.Exists(sb.ToString()))
        {
            using var newFile = File.CreateText(sb.ToString());
            newFile.Write(content);
            return;
        }

        using var writer = new StreamWriter(sb.ToString(), append);
        writer.Write(content);
    }

    public static string ReadText(string filePath, bool relativePath = true)
    {
        var sb = new StringBuilder();
        if (relativePath)
        {
            sb.Append(Application.persistentDataPath).Append("/");
        }
        sb.Append(filePath);

        if (!File.Exists(sb.ToString()))
        {
            return "";
        }

        using var reader = new StreamReader(sb.ToString());
        return reader.ReadToEnd();
    }
}

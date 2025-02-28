using Godot;

namespace Mimeva.Utils;
public partial class FileUtils
{

    // Appends <content> to the end of file <filename>
    public static void SaveToTxt(string filename, string content)
    {
        if (FileAccess.FileExists(filename))
        {
            using var file_r = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
            content = file_r.GetAsText() + "\n" + content;
            file_r.Close();
        }

        using var file = FileAccess.Open(filename, FileAccess.ModeFlags.Write);
        file.StoreString(content);
        file.Flush();
        return;
    }

    // Returns a string containing the contents of file <filename>
    public static string GetFromTxt(string filename)
    {
        using var file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
        return file.GetAsText();
    }

}

using Godot;

namespace Mimeva.Utils;
public partial class FileUtils
{

    // Appends <content> to the end of file <filename>
    public static void SaveToTxt(string filename, string content)
    {
        using var file = FileAccess.Open(filename, FileAccess.ModeFlags.Write);
        file.StoreString(content);
    }

}

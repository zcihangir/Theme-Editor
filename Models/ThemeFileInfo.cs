namespace Theme.Modern.Models;

public class ThemeFileInfo
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public bool IsReadOnly { get; set; }
    public DateTime LastModified { get; set; }
    public long Size { get; set; }
} 
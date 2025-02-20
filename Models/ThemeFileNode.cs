namespace Theme.Modern.Models;

public class ThemeFileNode
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string Path { get; set; }
    public string Type { get; set; }
    public string Extension { get; set; }
    public DateTime LastModified { get; set; }
    public long Size { get; set; }
    public List<ThemeFileNode> Children { get; set; } = new();
} 
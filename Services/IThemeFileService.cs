using Theme.Modern.Models;

namespace Theme.Modern.Services;

public interface IThemeFileService
{
    Task<List<ThemeFileNode>> GetContentFiles();
    Task<List<ThemeFileNode>> GetViewFiles();
    Task<string> GetFileContent(string filePath);
    Task SaveFileContent(string filePath, string content);
    Task<bool> DeleteFile(string filePath);
    Task<ThemeFileInfo> GetFileInfo(string filePath);
} 
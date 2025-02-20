using Grand.Infrastructure.Configuration;
using Microsoft.AspNetCore.Hosting;
using Theme.Modern.Models;
using System.IO;

namespace Theme.Modern.Services;

public class ThemeFileService : IThemeFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly AppConfig _config;

    public ThemeFileService(
        IWebHostEnvironment webHostEnvironment,
        AppConfig config)
    {
        _webHostEnvironment = webHostEnvironment;
        _config = config;
    }

    public async Task<List<ThemeFileNode>> GetContentFiles()
    {
        var contentPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Plugins", "Theme.Modern", "Content");
        if (!Directory.Exists(contentPath))
        {
            contentPath = Path.Combine(_webHostEnvironment.WebRootPath, "Plugins", "Theme.Modern", "Content");
        }
        return await GetDirectoryNodes(contentPath, "Content");
    }

    public async Task<List<ThemeFileNode>> GetViewFiles()
    {
        var viewsPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Plugins", "Theme.Modern", "Views");
        if (!Directory.Exists(viewsPath))
        {
            viewsPath = Path.Combine(_webHostEnvironment.WebRootPath, "Plugins", "Theme.Modern", "Views");
        }
        return await GetDirectoryNodes(viewsPath, "Views");
    }

    private async Task<List<ThemeFileNode>> GetDirectoryNodes(string path, string rootName)
    {
        var nodes = new List<ThemeFileNode>();
        if (!Directory.Exists(path))
            return nodes;

        var rootNode = new ThemeFileNode
        {
            Id = path.ToLower(),
            Text = rootName,
            Path = path,
            Type = "folder",
            Children = new List<ThemeFileNode>()
        };

        await ProcessDirectory(path, rootNode);
        nodes.Add(rootNode);
        return nodes;
    }

    private async Task ProcessDirectory(string path, ThemeFileNode parentNode)
    {
        foreach (var dir in Directory.GetDirectories(path))
        {
            var dirInfo = new DirectoryInfo(dir);
            var node = new ThemeFileNode
            {
                Id = dir.ToLower(),
                Text = dirInfo.Name,
                Path = dir,
                Type = "folder",
                Children = new List<ThemeFileNode>()
            };

            await ProcessDirectory(dir, node);
            parentNode.Children.Add(node);
        }

        foreach (var file in Directory.GetFiles(path))
        {
            var fileInfo = new FileInfo(file);
            parentNode.Children.Add(new ThemeFileNode
            {
                Id = file.ToLower(),
                Text = fileInfo.Name,
                Path = file,
                Type = "file",
                Extension = fileInfo.Extension,
                LastModified = fileInfo.LastWriteTime,
                Size = fileInfo.Length
            });
        }
    }

    public async Task<string> GetFileContent(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Dosya bulunamadı: {filePath}");

        return await File.ReadAllTextAsync(filePath);
    }

    public async Task SaveFileContent(string filePath, string content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, content);

        if (Path.GetExtension(filePath).Equals(".cshtml", StringComparison.OrdinalIgnoreCase))
        {
            var tempViewPath = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data/TempViews");
            if (Directory.Exists(tempViewPath))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var tempFiles = Directory.GetFiles(tempViewPath, "*.compiled")
                    .Where(f => f.Contains(fileName));

                foreach (var tempFile in tempFiles)
                {
                    try { File.Delete(tempFile); } catch { }
                }
            }
        }
    }

    public async Task<bool> DeleteFile(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        File.Delete(filePath);
        return true;
    }

    public async Task<ThemeFileInfo> GetFileInfo(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Dosya bulunamadı: {filePath}");

        var fileInfo = new FileInfo(filePath);
        return new ThemeFileInfo
        {
            Name = fileInfo.Name,
            Path = filePath,
            Extension = fileInfo.Extension,
            IsReadOnly = fileInfo.IsReadOnly,
            LastModified = fileInfo.LastWriteTime,
            Size = fileInfo.Length
        };
    }
} 
using Grand.Business.Core.Interfaces.Common.Security;
using Grand.Web.Common.Controllers;
using Microsoft.AspNetCore.Mvc;
using Theme.Modern.Models;
using Theme.Modern.Services;
using Grand.Domain.Permissions;

namespace Theme.Modern.Areas.Admin.Controllers;

public class ThemeModernController : BaseAdminPluginController
{
    private readonly IPermissionService _permissionService;
    private readonly IThemeFileService _themeFileService;

    public ThemeModernController(
        IPermissionService permissionService,
        IThemeFileService themeFileService)
    {
        _permissionService = permissionService;
        _themeFileService = themeFileService;
    }

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.Authorize(StandardPermission.ManagePlugins))
            return AccessDeniedView();

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetContentFiles()
    {
        if (!await _permissionService.Authorize(StandardPermission.ManagePlugins))
            return AccessDeniedView();

        try
        {
            var nodes = await _themeFileService.GetContentFiles();
            return Json(nodes.FirstOrDefault()?.Children ?? new List<ThemeFileNode>());
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetViewFiles()
    {
        if (!await _permissionService.Authorize(StandardPermission.ManagePlugins))
            return AccessDeniedView();

        try
        {
            var nodes = await _themeFileService.GetViewFiles();
            return Json(nodes.FirstOrDefault()?.Children ?? new List<ThemeFileNode>());
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetFileContent(string filePath)
    {
        if (!await _permissionService.Authorize(StandardPermission.ManagePlugins))
            return AccessDeniedView();

        try
        {
            if (string.IsNullOrEmpty(filePath))
                return Json(new { success = false, message = "Geçersiz dosya yolu" });

            var extension = Path.GetExtension(filePath).ToLower();
            var isBinaryFile = new[] { ".jpg", ".jpeg", ".png", ".gif", ".ttf", ".woff", ".woff2", ".eot" }.Contains(extension);

            if (isBinaryFile)
            {
                return Json(new { 
                    success = true, 
                    content = "Bu dosya türü görüntülenemez.", 
                    fileInfo = await _themeFileService.GetFileInfo(filePath),
                    isBinary = true 
                });
            }

            var content = await _themeFileService.GetFileContent(filePath);
            var fileInfo = await _themeFileService.GetFileInfo(filePath);

            return Json(new { success = true, content, fileInfo, isBinary = false });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveFile(string filePath, string content)
    {
        if (!await _permissionService.Authorize(StandardPermission.ManagePlugins))
            return AccessDeniedView();

        try
        {
            if (string.IsNullOrEmpty(filePath))
                return Json(new { success = false, message = "Dosya yolu boş olamaz" });

            if (content == null)
                return Json(new { success = false, message = "Dosya içeriği boş olamaz" });

            var extension = Path.GetExtension(filePath).ToLower();
            var isBinaryFile = new[] { ".jpg", ".jpeg", ".png", ".gif", ".ttf", ".woff", ".woff2", ".eot" }.Contains(extension);

            if (isBinaryFile)
                return Json(new { success = false, message = "Bu dosya türü düzenlenemez" });

            await _themeFileService.SaveFileContent(filePath, content);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFile(string filePath)
    {
        if (!await _permissionService.Authorize(StandardPermission.ManagePlugins))
            return AccessDeniedView();

        try
        {
            var result = await _themeFileService.DeleteFile(filePath);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
} 
using Grand.Infrastructure;
using Grand.Web.Common.Themes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Theme.Modern.Services;

namespace Theme.Modern;

public class StartupApplication : IStartupApplication
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IThemeView, ModernThemeView>();
        services.AddScoped<IThemeFileService, ThemeFileService>();
    }

    public int Priority => 100;

    public void Configure(WebApplication application, IWebHostEnvironment webHostEnvironment)
    {
    }

    public bool BeforeConfigure => false;
}
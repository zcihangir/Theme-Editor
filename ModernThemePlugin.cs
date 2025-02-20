using Grand.Infrastructure.Plugins;

namespace Theme.Modern;

/// <summary>
///     Plugin
/// </summary>
public class MinimalThemePlugin : BasePlugin, IPlugin
{
    public override string ConfigurationUrl()
    {
        return "../ThemeModern/Configure";
    }
}



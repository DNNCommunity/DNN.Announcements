using DotNetNuke.Modules.Announcements.Components.Common;

namespace DotNetNuke.Modules.Announcements.Components.Settings
{
    public interface ISettingsController
    {
        Settings GetModuleSettings(int moduleId, int tabModuleId);
    }
}
using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Announcements.Components.Common;

namespace DotNetNuke.Modules.Announcements.Components.Settings
{
    public class SettingsController : ISettingsController
    {
        public Settings GetModuleSettings(int moduleId, int tabModuleId)
        {
            return CBO.GetCachedObject<Settings>(new CacheItemArgs(CacheConstants.SettingsCacheKeyFormat(moduleId, tabModuleId), CacheConstants.SettingsCacheTimeOut, CacheConstants.SettingsCachePriority, moduleId, tabModuleId), GetSettingsCallback);
        }

        private object GetSettingsCallback(CacheItemArgs cacheItemArgs)
        {
            var moduleId = (int)cacheItemArgs.ParamList[0];
            var tabModuleId = (int)cacheItemArgs.ParamList[1];
            return new Settings(moduleId, tabModuleId);

        }


    }
}
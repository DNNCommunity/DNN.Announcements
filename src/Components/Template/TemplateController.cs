using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Modules.Announcements.Components.Settings;

namespace DotNetNuke.Modules.Announcements.Components.Template
{
    public class TemplateController : ITemplateController
    {
        public ITemplate GetTemplate(int moduleId, int tabModuleId)
        {
            return CBO.GetCachedObject<ITemplate>(new CacheItemArgs(CacheConstants.TemplateCacheKeyFormat(tabModuleId), CacheConstants.TemplateCacheTimeOut, CacheConstants.TemplateCachePriority, moduleId, tabModuleId), GetTemplateCallback);
        }

        /// <summary>
        /// Callback method that dynamically loads a type of template, using reflection
        /// </summary>
        /// <param name="cacheItemArgs">Arguments, containing moduleid and tabModuleId </param>
        /// <returns>ITemplate</returns>
        private object GetTemplateCallback(CacheItemArgs cacheItemArgs)
        {
            var moduleId = (int)cacheItemArgs.ParamList[0];
            var tabModuleId = (int)cacheItemArgs.ParamList[1];
            var moduleSettings = new SettingsController().GetModuleSettings(moduleId, tabModuleId);


                Type type = Type.GetType("DotNetNuke.Modules.Announcements.Components.Template." + moduleSettings.TemplateType);
                if (type != null && type.IsClass)
                {
                    var template = Activator.CreateInstance(type,
                        new object[] { moduleId, tabModuleId, moduleSettings.TemplateName });
                    return template;
                }
            return null;

        }

    }
}
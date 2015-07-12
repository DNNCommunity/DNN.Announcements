#region License

//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

#endregion

using System.Web.Caching;

namespace DotNetNuke.Modules.Announcements.Components.Common
{
    public class ModuleDefinition
    {
        public const string FriendlyName = "Announcements";
    }

    public class CacheConstants
    {
        private const string _TemplateCacheKey = "dnnAnnouncements_Template_{0}";
        public static string TemplateCacheKeyFormat(int tabModuleId)
        {
            return string.Format(_TemplateCacheKey, tabModuleId);
        }
        public const int TemplateCacheTimeOut =  20;
        public const CacheItemPriority TemplateCachePriority = CacheItemPriority.Normal;
        private const string _ModuleSettingsCacheKey = "dnnAnnouncements_Settings_{0}_{1}";
        public static string SettingsCacheKeyFormat(int moduleId, int tabModuleId)
        {
            return string.Format(_ModuleSettingsCacheKey, moduleId, tabModuleId);
        }
        public const int SettingsCacheTimeOut = 20;
        public const CacheItemPriority SettingsCachePriority = CacheItemPriority.Normal;
    }

    public class PermissionName
    {
        public const string Code = "DNNANNOUNCEMENTS";
        public const string HasEditTemplatePermission = "EDITTEMPLATE";
        public const string HasSubscribePermission = "CANSUBSCRIBE";
    }

    public class SettingName
    {
        public const string TemplateType = "SimpleTemplate";
        public const string History = "history";
        public const string DescriptionLength = "descriptionLength";
        public const string EditorHeight = "editorHeight";
        public const string DefaultViewType = "defaultViewType";
        public const string ConnectedModule = "ConnectedModule";
        public const string Legacy = "Legacy";
        public const string TemplateName = "TemplateName";
        public const string TemplateLocation = "TemplateLocation";

        //legacy
        public const string ItemTemplate = "template";
        public const string AltItemTemplate = "altitemtemplate";
        public const string Separator = "separator";
        public const string HeaderTemplate = "headertemplate";
        public const string FooterTemplate = "footertemplate";

    }

    public class JournalType
    {
        public const string AnnouncementAdd = "AnnouncementAdd";
        public const string AnnouncementUpdate = "AnnouncementUpdate";
    }

    public class NotificationType
    {
        public const string Announcements = "DNNAnnouncements";
    }

}
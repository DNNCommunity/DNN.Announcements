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

#region Usings

using System;
using System.Collections;
using System.Globalization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Announcements.Components.Common;

#endregion

namespace DotNetNuke.Modules.Announcements.Components.Settings
{
    public class Settings
    {
        #region constants
        private readonly int _History = Null.NullInteger;
        private const int _DescriptionLength = 100;
        private const int _EditorHeight = 300;
        private readonly string _DefaultViewType = Utilities.ViewTypeToString(ViewTypes.Current);
        private const bool _Legacy = false;
        private const string _TemplateType = "SimpleTemplate";
        private const string _TemplateName = "ModernAnnouncements";
        private const string _TemplateLocation = "system";
        #endregion

        #region private properties
        private readonly int _moduleId = -1;
        private readonly int _tabModuleId = -1;
        private readonly Hashtable ModuleSettings;
        private readonly Hashtable TabModuleSettings;
        #endregion

        #region public properties
        public int History { get; set; }
        public int DescriptionLength { get; set; }
        public int EditorHeight { get; set; }
        public ViewTypes DefaultViewType { get; set; }
        public bool Legacy { get; set; }
        public string TemplateType { get; set; }
        public string TemplateName { get; set; }
        public string TemplateLocation { get; set; }

        // legacy settings
        public string ItemTemplate { get; set; }
        public string AltItemTemplate { get; set; }
        public string Separator { get; set; }
        public string HeaderTemplate { get; set; }
        public string FooterTemplate { get; set; }
        #endregion

        #region constructors
        public Settings(int moduleId, int tabModuleId)
        {
            _moduleId = moduleId;
            _tabModuleId = tabModuleId;

            var moduleInfo = new ModuleController().GetModule(_moduleId);
            ModuleSettings = moduleInfo.ModuleSettings;
            TabModuleSettings = moduleInfo.TabModuleSettings;

            History = TabModuleSettings.GetInteger(SettingName.History, _History);
            DescriptionLength = TabModuleSettings.GetInteger(SettingName.DescriptionLength, _DescriptionLength);
            EditorHeight = TabModuleSettings.GetInteger(SettingName.EditorHeight, _EditorHeight);
            DefaultViewType = Utilities.StringToViewType(TabModuleSettings.GetString(SettingName.DefaultViewType,_DefaultViewType));
            TemplateType = TabModuleSettings.GetString(SettingName.TemplateType, _TemplateType);
            TemplateName = TabModuleSettings.GetString(SettingName.TemplateName, _TemplateName);
            TemplateLocation = TabModuleSettings.GetString(SettingName.TemplateLocation, _TemplateLocation);
            
            
            //legacy
            ItemTemplate = TabModuleSettings.GetString(SettingName.ItemTemplate, Null.NullString);
            AltItemTemplate = TabModuleSettings.GetString(SettingName.AltItemTemplate, Null.NullString);
            Separator = TabModuleSettings.GetString(SettingName.HeaderTemplate, Null.NullString);
            HeaderTemplate = TabModuleSettings.GetString(SettingName.HeaderTemplate, Null.NullString);
            FooterTemplate = TabModuleSettings.GetString(SettingName.FooterTemplate, Null.NullString);

            Legacy = ModuleSettings.GetBoolean(SettingName.Legacy, _Legacy) || !Legacy && !string.IsNullOrEmpty(ItemTemplate);
        }
        #endregion


        public void Update()
        {
            var objModules = new ModuleController();

            objModules.UpdateTabModuleSetting(_tabModuleId, SettingName.History, History.ToString(CultureInfo.InvariantCulture));
            objModules.UpdateModuleSetting(_moduleId, SettingName.DescriptionLength, DescriptionLength.ToString(CultureInfo.InvariantCulture));
            objModules.UpdateTabModuleSetting(_tabModuleId, SettingName.EditorHeight, EditorHeight.ToString(CultureInfo.InvariantCulture));
            objModules.UpdateTabModuleSetting(_tabModuleId, SettingName.DefaultViewType, Utilities.ViewTypeToString(DefaultViewType));
            objModules.UpdateModuleSetting(_moduleId, SettingName.Legacy, Legacy.ToString(CultureInfo.InvariantCulture));
            objModules.UpdateTabModuleSetting(_tabModuleId, SettingName.TemplateType, TemplateType);
            objModules.UpdateTabModuleSetting(_tabModuleId, SettingName.TemplateName, TemplateName);
            objModules.UpdateTabModuleSetting(_tabModuleId, SettingName.TemplateLocation, TemplateLocation);


            ModuleController.SynchronizeModule(_moduleId);
            // DataCache.RemoveCache(ModuleController.CacheKey(_moduleId) + "_viewType");
            // Module caching has been updated in 5.2.0, this method is no longer used

            DataCache.RemoveCache(CacheConstants.SettingsCacheKeyFormat(_moduleId, _tabModuleId));

        }

        public bool TryGetValue(string key, out string value)
        {
            bool returnValue = false;
            string outValue = Null.NullString;
            try
            {

                if (ModuleSettings != null)
                {
                    if (ModuleSettings.ContainsKey(key))
                    {
                        outValue = ModuleSettings[key] as string;
                        returnValue = true;
                    }
                }
                if (Null.IsNull(outValue))
                {
                    if (TabModuleSettings != null)
                    {
                        if (TabModuleSettings.ContainsKey(key))
                        {
                            outValue = TabModuleSettings[key] as string;
                            returnValue = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                outValue = Null.NullString;
                returnValue = false;
            }

            value = outValue;
            return returnValue;

        }

    }
}
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Xml.Serialization;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.Modules.Announcements.Components.Template
{
    [Serializable]
	public class Template
	{

        #region  Private Members

        private string _localResourceFile = Globals.ApplicationPath + "/DesktopModules/Announcements/App_LocalResources/AnnouncementsView.ascx";
        private Dictionary<string, string> _settings;

        private int _tabModuleId = -1;

        #endregion


        #region  Public Properties

        /// <summary>
        /// Gets and sets the name of the template
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets and sets the Item Template
        /// </summary>
        public string ItemTemplate { get; set; }

        /// <summary>
        /// Gets and sets the alternate item template
        /// </summary>
        public string AltItemTemplate { get; set; }

        /// <summary>
        /// Gets and sets the Separator
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Gets and sets the HeaderTemplate
        /// </summary>
        public string HeaderTemplate { get; set; }

        /// <summary>
        /// Gets and sets the FooterTemplate
        /// </summary>
        public string FooterTemplate { get; set; }

        public bool TokenReplaceNeeded
        {
            get
            {
                return (!(string.IsNullOrEmpty(HeaderTemplate))) | (!(string.IsNullOrEmpty(FooterTemplate))) | (!(string.IsNullOrEmpty(Separator)));
            }
        }
        #endregion


        public string GetTemplate(string templateName, string settingsName)
        {
            string _setting;
            if (_settings.TryGetValue(settingsName.ToLowerInvariant(), out _setting))
            {
                // legacy template
                return Convert.ToString(_setting);
            }
            return GetTemplate(templateName, settingsName, true);
        }

        public string GetTemplate(string templateName, string settingsName, bool forceDefault)
        {

            if (!string.IsNullOrEmpty(settingsName))
            {
                if (File.Exists(TemplatePath + settingsName))
                {
                    return File.ReadAllText(TemplatePath + settingsName);
                }
            }
            return "";
        }

        private string _templatePath;
        private string TemplatePath
        {
            get
            {
                return _templatePath ?? (_templatePath = HttpContext.Current.Server.MapPath(
                    Globals.ResolveUrl(string.Format(@"~/DesktopModules/Announcements/Templates/{0}/", Name))));
            }
        }

        public Template(int tabModuleId, Dictionary<string, string> settings)
        {
            Name = "SimpleAnnouncements";
            _settings = settings;
            _tabModuleId = tabModuleId;

            TemplateInfo templateInfo;
            XmlSerializer serializer = new XmlSerializer(typeof(TemplateInfo));
            using (StreamReader reader = new StreamReader(TemplatePath + "Template.xml"))
            {
                templateInfo = (TemplateInfo)serializer.Deserialize(reader);
            }


            ItemTemplate = GetTemplate(Name, templateInfo.ItemTemplate);
            AltItemTemplate = GetTemplate(Name, templateInfo.AltItemTemplate);
            Separator = GetTemplate(Name, templateInfo.SeparatorTemplate);
            HeaderTemplate = GetTemplate(Name, templateInfo.HeaderTemplate);
            FooterTemplate = GetTemplate(Name, templateInfo.FooterTemplate);

        }

        public void UpdateTemplate()
        {
            //var moduleController = new Entities.Modules.ModuleController();
            //moduleController.UpdateTabModuleSetting(_tabModuleId, "template", ItemTemplate);
            //moduleController.UpdateTabModuleSetting(_tabModuleId, "altitemtemplate", AltItemTemplate);
            //moduleController.UpdateTabModuleSetting(_tabModuleId, "separator", Separator);
            //moduleController.UpdateTabModuleSetting(_tabModuleId, "headertemplate", HeaderTemplate);
            //moduleController.UpdateTabModuleSetting(_tabModuleId, "footertemplate", FooterTemplate);

            //DataCache.RemoveCache(TemplateCachKey);

        }

    }
}

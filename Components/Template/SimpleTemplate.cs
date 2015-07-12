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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Tokens;

#endregion

namespace DotNetNuke.Modules.Announcements.Components.Template
{

    public class SimpleTemplate : BaseTemplate
    {

        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(SimpleTemplate));

        #region  Public Properties
        public string ItemTemplate { get; set; }
        public string AltItemTemplate { get; set; }
        public string Separator { get; set; }
        public string HeaderTemplate { get; set; }
        public string FooterTemplate { get; set; }

        private bool TokenReplaceNeeded
        {
            get
            {
                return (!(string.IsNullOrEmpty(HeaderTemplate))) | (!(string.IsNullOrEmpty(FooterTemplate))) | (!(string.IsNullOrEmpty(Separator)));
            }
        }
        #endregion

        #region Private Methods
        private string GetTemplate(string settingName)
        {
            if (Legacy)
            {
                string setting;
                if (Settings.TryGetValue(settingName.ToLowerInvariant(), out setting))
                {
                    // legacy template
                    return Convert.ToString(setting);
                }
                Logger.ErrorFormat("legacy template expected, but not found, settingName: {0}, loading new style template", settingName);
            }
            return GetTemplate(settingName, true);
        }

        private string GetTemplate(string settingName, bool forceDefault)
        {
            if (!string.IsNullOrEmpty(settingName))
            {
                if (File.Exists(TemplatePath + settingName))
                {
                    return File.ReadAllText(TemplatePath + settingName, Encoding.UTF8);
                }
                Logger.ErrorFormat("error loading template, file does not exist: {0}", TemplatePath + settingName);
            }
            else
            {
                Logger.ErrorFormat("error loading template, settingname empty {0}", settingName);
            }
            return "";
        }

        private void WriteTemplate(string settingName, string value)
        {
            if (!string.IsNullOrEmpty(settingName))
            {
                using (var streamWriter = new StreamWriter(File.Open(TemplatePath + settingName, FileMode.Create), Encoding.UTF8))
                {
                    streamWriter.Write(value);
                }
            }
        }

        #endregion

        public SimpleTemplate(int moduleId, int tabModuleId, string templateName)
            : base(moduleId, tabModuleId, templateName)
        {

        }

        public override void LoadTemplate()
        {
            base.LoadTemplate();

            ItemTemplate = GetTemplate(TemplateInfo.ItemTemplate);
            AltItemTemplate = GetTemplate(TemplateInfo.AltItemTemplate);
            Separator = GetTemplate(TemplateInfo.SeparatorTemplate);
            HeaderTemplate = GetTemplate(TemplateInfo.HeaderTemplate);
            FooterTemplate = GetTemplate(TemplateInfo.FooterTemplate);
            JsFile = GetTemplate(TemplateInfo.JsFile);
            CssFile = GetTemplate(TemplateInfo.CssFile);

        }


        public override void UpdateTemplate()
        {
            WriteTemplate(TemplateInfo.ItemTemplate, ItemTemplate);
            WriteTemplate(TemplateInfo.AltItemTemplate, AltItemTemplate);
            WriteTemplate(TemplateInfo.SeparatorTemplate, Separator);
            WriteTemplate(TemplateInfo.HeaderTemplate, HeaderTemplate);
            WriteTemplate(TemplateInfo.FooterTemplate, FooterTemplate);
            WriteTemplate(TemplateInfo.JsFile, JsFile);
            WriteTemplate(TemplateInfo.CssFile, CssFile);
            //DataCache.RemoveCache(TemplateCachKey);

        }

        public override string RenderTemplate(IEnumerable<AnnouncementInfo> announcements, bool editEnabled)
        {
            var output = new StringBuilder();


            TokenReplace dnnTokenReplace = null;
            if (TokenReplaceNeeded)
            {
                dnnTokenReplace = new TokenReplace(Scope.DefaultSettings, CultureInfo.CurrentCulture.Name, PortalSettings, PortalSettings.UserInfo);
            }
            bool altItemTemplateAvailable = !(string.IsNullOrEmpty(AltItemTemplate));
            if (dnnTokenReplace != null)
            {
                output.Append(dnnTokenReplace.ReplaceEnvironmentTokens(HeaderTemplate));
            }
            int counter = 0;
            var tokenReplace = new AnnouncementsTokenReplace();
            var announcementInfos = announcements as IList<AnnouncementInfo> ?? announcements.ToList();
            foreach (var announcement in announcementInfos)
            {
                //we have to pass IsEditable to the announcement, because it is used to draw the edit icon
                announcement.IsEditable = editEnabled;

                //Create a Token Replace and replace the tokens for this template
                tokenReplace.SetPropertySource(announcement);
                if ((counter % 2 == 0) | (!altItemTemplateAvailable))
                {
                    output.Append(tokenReplace.ReplaceAnnouncmentTokens(ItemTemplate));
                }
                else
                {
                    output.Append(tokenReplace.ReplaceAnnouncmentTokens(AltItemTemplate));
                }
                if ((dnnTokenReplace != null) && (counter < announcementInfos.Count() - 1))
                {
                    output.Append(dnnTokenReplace.ReplaceEnvironmentTokens(Separator));
                }
                counter += 1;

            }
            if (dnnTokenReplace != null)
            {
                output.Append(dnnTokenReplace.ReplaceEnvironmentTokens(FooterTemplate));
            }

            return output.ToString();
        }
    }
}

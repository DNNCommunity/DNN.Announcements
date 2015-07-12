using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Modules.Announcements.Components.Settings;

namespace DotNetNuke.Modules.Announcements.Components.Template
{
    public abstract class BaseTemplate : ITemplate
    {

        protected BaseTemplate(int moduleId, int tabModuleId, string templateName)
        {
            ModuleId = moduleId;
            TabModuleId = tabModuleId;
            _settings = new SettingsController().GetModuleSettings(ModuleId, TabModuleId);
            Name = templateName;

        }

        public virtual void LoadTemplate()
        {
            var serializer = new XmlSerializer(typeof(TemplateInfo));
            using (var reader = new StreamReader(TemplatePath + "Template.xml"))
            {
                TemplateInfo = (TemplateInfo)serializer.Deserialize(reader);
            }
        }

        public abstract void UpdateTemplate();

        public abstract string RenderTemplate(IEnumerable<AnnouncementInfo> announcements, bool editEnabled);

        protected string ModuleResourceFile = Globals.ApplicationPath + "/DesktopModules/Announcements/App_LocalResources/AnnouncementsView.ascx";

        private string _templatePath;
        protected string TemplatePath
        {
            get
            {
                return _templatePath ?? (_templatePath = HttpContext.Current.Server.MapPath(
                    Globals.ResolveUrl(string.Format(@"~/DesktopModules/Announcements/Templates/{0}/", Name))));
            }
        }

        private Settings.Settings _settings;
        protected Settings.Settings Settings
        {
            get { return _settings ?? (_settings = new SettingsController().GetModuleSettings(ModuleId, TabModuleId)); }
        }

        protected bool Legacy
        {
            get { return Settings.Legacy; }
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public int ModuleId { get; set; }
        public int TabModuleId { get; set; }
        public TemplateInfo TemplateInfo { get; set; }
        public string JsFile { get; set; }
        public string CssFile { get; set; }

        protected PortalSettings PortalSettings
        {
            get { return Globals.GetPortalSettings(); }
        }
    }
}
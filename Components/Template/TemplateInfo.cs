using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.Modules.Announcements.Components.Template
{
    public class TemplateInfo
    {
        public string Type { get; set; }
        public string ItemTemplate { get; set; }
        public string AltItemTemplate { get; set; }
        public string HeaderTemplate { get; set; }
        public string FooterTemplate { get; set; }
        public string SeparatorTemplate { get; set; }
        public string DetailTemplate { get; set; }
        public string CssFile { get; set; }
        public string JsFile { get; set; }
        public string Version { get; set; }
        public DateTime LastModifiedDate { get; set; }

    }
}
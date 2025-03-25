using DotNetNuke.Modules.Announcements.Components.Settings;
using DotNetNuke.Modules.Announcements.Components.Template;

namespace DotNetNuke.Modules.Announcements.MVP.Models
{
    public class TemplateConfigurationModel
    {
        public Settings Settings { get; set; }
        public ITemplate Template { get; set; }

    }
}
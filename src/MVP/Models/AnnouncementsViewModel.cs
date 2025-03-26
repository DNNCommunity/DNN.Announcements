using System.Collections.Generic;

using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Modules.Announcements.Components.Settings;
using DotNetNuke.Modules.Announcements.Components.Template;

namespace DotNetNuke.Modules.Announcements.MVP.Models
{
    public class AnnouncementsViewModel
    {
        public Settings Settings { get; set; }
        public ITemplate Template { get; set; }
        public bool EditEnabled { get; set; }
        public bool CanEdit { get; set; }
        public bool CanSubscribe { get; set; }
        public IEnumerable<AnnouncementInfo> Announcements { get; set; }
        public string RenderedTemplate { get; set; }
    }
}
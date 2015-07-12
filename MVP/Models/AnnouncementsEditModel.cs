using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Modules.Announcements.Components.Settings;

namespace DotNetNuke.Modules.Announcements.MVP.Models
{
    public class AnnouncementsEditModel
    {
        public Settings Settings { get; set; }
        public int ItemId { get; set; }
        public AnnouncementInfo AnnouncementInfo { get; set; }
        public bool IsAddMode { get; set; }
        public bool ItemIdError { get; set; }
    }
}
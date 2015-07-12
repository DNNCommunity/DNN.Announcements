using System;

using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Web.Mvp;

namespace DotNetNuke.Modules.Announcements.MVP.Views
{
    public interface IAnnouncementsView : IModuleView<AnnouncementsViewModel>
    {
        event EventHandler GetSettings;
        event EventHandler GetAnnouncements;
        event EventHandler GetPermissions;
        event EventHandler GetTemplate;
        event EventHandler GetRenderedTemplate;

    }
}
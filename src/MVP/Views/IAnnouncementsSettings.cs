using System;

using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Web.Mvp;

namespace DotNetNuke.Modules.Announcements.MVP.Views
{
    public interface IAnnouncementsSettings : ISettingsView<AnnouncementsSettingsModel>
    {
        event EventHandler GetSettings;

    }
}
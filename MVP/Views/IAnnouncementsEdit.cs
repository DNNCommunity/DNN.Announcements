using System;

using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Web.Mvp;

namespace DotNetNuke.Modules.Announcements.MVP.Views
{
    public interface IAnnouncementsEdit : IModuleView<AnnouncementsEditModel>
    {
        event EventHandler GetSettings;
        event EventHandler<EditItemEventArgs> GetItem;
        event EventHandler GetAnnouncement;
        event EventHandler<EditItemEventArgs> DeleteAnnouncement;
        event EventHandler<EditItemEventArgs> UpdateAnnouncement;
    }
}
using System;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Modules.Announcements.Components.Settings;
using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Modules.Announcements.MVP.Views;
using DotNetNuke.Web.Mvp;

namespace DotNetNuke.Modules.Announcements.MVP.Presenters
{
    public class AnnouncementsEditPresenter : ModulePresenter<IAnnouncementsEdit, AnnouncementsEditModel>
    {
        private readonly IAnnouncementsController _announcementsController;

        public AnnouncementsEditPresenter(IAnnouncementsEdit announcementsEdit)
            : this(announcementsEdit, new AnnouncementsController())
        {
        }

        public AnnouncementsEditPresenter(IAnnouncementsEdit announcementsEdit, IAnnouncementsController announcementsController)
            : base(announcementsEdit)
        {
            Requires.NotNull("announcementsController", announcementsController);
            _announcementsController = announcementsController;

            View.GetSettings += GetSettings;
            View.GetItem += GetItem;
            View.GetAnnouncement += GetAnnouncement;
            View.DeleteAnnouncement += DeleteAnnouncement;
            View.UpdateAnnouncement += UpdateAnnouncement;
        }

        private void UpdateAnnouncement(object sender, EditItemEventArgs e)
        {
            if (View.Model.IsAddMode)
            {
                _announcementsController.AddAnnouncement(e.Item);
                _announcementsController.AddAnnouncementToJournal(e.Item, JournalType.AnnouncementAdd, ModuleInfo);
                _announcementsController.SendNotifications(e.Item);
            }
            else
            {
                _announcementsController.UpdateAnnouncement(e.Item);
                _announcementsController.AddAnnouncementToJournal(e.Item, JournalType.AnnouncementUpdate, ModuleInfo);
            }

        }

        private void DeleteAnnouncement(object sender, EditItemEventArgs e)
        {
            if (!Null.IsNull(e.ItemId))
            {
                _announcementsController.DeleteAnnouncement(ModuleId, e.ItemId);
            }
        }

        private void GetSettings(object sender, EventArgs e)
        {
            Requires.NotNull("ModuleContext", ModuleContext);
            View.Model.Settings = new SettingsController().GetModuleSettings(ModuleContext.ModuleId, ModuleContext.TabModuleId);
        }

        private void GetItem(object sender, EditItemEventArgs e)
        {
            int resultValue;
            if (!int.TryParse(e.ItemIdString, out resultValue))
            {
                View.Model.ItemIdError = e.ItemIdString != null;
                resultValue = Null.NullInteger;
            }
            View.Model.ItemId = resultValue;
            View.Model.IsAddMode = Null.IsNull(resultValue) && !View.Model.ItemIdError;
        }

        private void GetAnnouncement(object sender, EventArgs e)
        {
            if (!View.Model.IsAddMode && !Null.IsNull(View.Model.ItemId))
            {
                View.Model.AnnouncementInfo = _announcementsController.GetAnnouncement(View.Model.ItemId, ModuleId);   
            }
        }

    }
}
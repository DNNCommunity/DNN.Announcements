using System;

using DotNetNuke.Common;
using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Modules.Announcements.Components.Settings;
using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Modules.Announcements.MVP.Views;
using DotNetNuke.Web.Mvp;

namespace DotNetNuke.Modules.Announcements.MVP.Presenters
{
    public class AnnouncementsSettingsPresenter : ModuleSettingsPresenter<IAnnouncementsSettings, AnnouncementsSettingsModel>
    {
        private readonly IAnnouncementsController _announcementsController;

        public AnnouncementsSettingsPresenter(IAnnouncementsSettings announcementsSettings)
            : base(announcementsSettings)
        {
            View.GetSettings += GetSettings;

        }

        private void GetSettings(object sender, EventArgs e)
        {
            Requires.NotNull("ModuleContext", ModuleContext);
            View.Model.Settings = new SettingsController().GetModuleSettings(ModuleContext.ModuleId, ModuleContext.TabModuleId);
        }
    }
}
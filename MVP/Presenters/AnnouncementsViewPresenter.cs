using System;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Modules.Announcements.Components.Settings;
using DotNetNuke.Modules.Announcements.Components.Template;
using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Modules.Announcements.MVP.Views;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Web.Mvp;

namespace DotNetNuke.Modules.Announcements.MVP.Presenters
{
    public class AnnouncementsViewPresenter : ModulePresenter<IAnnouncementsView, AnnouncementsViewModel>
    {
        private readonly IAnnouncementsController _announcementsController;

        public AnnouncementsViewPresenter(IAnnouncementsView announcementsView)
            : this(announcementsView, new AnnouncementsController())
        {
        }

        public AnnouncementsViewPresenter(IAnnouncementsView announcementsView, IAnnouncementsController announcementsController)
            : base(announcementsView)
        {
            Requires.NotNull("announcementsController", announcementsController);

            _announcementsController = announcementsController;

            View.GetSettings += GetSettings;
            View.GetAnnouncements += GetAnnouncements;
            View.GetTemplate += GetTemplate;
            View.GetPermissions += GetPermissions;
            View.GetRenderedTemplate += GetRenderedTemplate;

        }

        private void GetRenderedTemplate(object sender, EventArgs e)
        {
            View.Model.Template.LoadTemplate();
            View.Model.RenderedTemplate = View.Model.Template.RenderTemplate(View.Model.Announcements, View.Model.EditEnabled);
        }

        private void GetSettings(object sender, EventArgs e)
        {
            Requires.NotNull("ModuleContext", ModuleContext);
            View.Model.Settings = new SettingsController().GetModuleSettings(ModuleContext.ModuleId, ModuleContext.TabModuleId);
        }

        private void GetPermissions(object sender, EventArgs e)
        {
            //View.Model.CanEdit = PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, ModuleContext.PortalSettings, ModuleInfo, ModuleContext.PortalSettings.UserInfo);
            View.Model.CanEdit = ModulePermissionController.CanEditModuleContent(ModuleInfo);
            View.Model.EditEnabled = (ModuleContext.PortalSettings.UserMode != PortalSettings.Mode.View) && View.Model.CanEdit;
            View.Model.CanSubscribe = ModulePermissionController.HasModulePermission(ModuleInfo.ModulePermissions, PermissionName.HasSubscribePermission);
        }

        private void GetTemplate(object sender, EventArgs e)
        {
            Requires.NotNull("ModuleContext", ModuleContext);
            View.Model.Template = new TemplateController().GetTemplate(ModuleContext.ModuleId, ModuleContext.TabModuleId);
        }


        /// <summary>
        /// Handler for the GetAnnouncements event. This handler sets a value to View.Model.Announcements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetAnnouncements(object sender, EventArgs e)
        {

            // bind data
            DateTime datStartDate = Null.NullDate;
            if (!View.Model.Settings.History.IsDnnNull())
            {
                datStartDate = DateTime.UtcNow.AddDays(-View.Model.Settings.History);
            }

            switch (View.Model.Settings.DefaultViewType)
            {
                case ViewTypes.Current:
                    View.Model.Announcements = _announcementsController.GetCurrentAnnouncements(ModuleId, datStartDate);
                    break;
                case ViewTypes.Expired:
                    View.Model.Announcements = _announcementsController.GetExpiredAnnouncements(ModuleId);
                    break;
                case ViewTypes.Future:
                    View.Model.Announcements = _announcementsController.GetAnnouncements(ModuleId, DateTime.UtcNow, Null.NullDate);
                    break;
                case ViewTypes.All:
                    View.Model.Announcements = _announcementsController.GetAnnouncements(ModuleId, Null.NullDate, Null.NullDate);
                    break;
            }


        }

    }
}
using System;
using System.IO;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Modules.Announcements.Components.Template;
using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Modules.Announcements.MVP.Presenters;
using DotNetNuke.Modules.Announcements.MVP.Views;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Mvp;

using WebFormsMvp;

namespace DotNetNuke.Modules.Announcements
{
    [PresenterBinding(typeof(AnnouncementsViewPresenter))]
    public partial class AnnouncementsView : ModuleView<AnnouncementsViewModel>, IAnnouncementsView,  IActionable
    {
        public event EventHandler GetSettings;
        public event EventHandler GetAnnouncements;
        public event EventHandler GetPermissions;
        public event EventHandler GetTemplate;
        public event EventHandler GetRenderedTemplate;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GetSettings(this, EventArgs.Empty);
            GetPermissions(this, EventArgs.Empty);
            GetAnnouncements(this, EventArgs.Empty);
            GetTemplate(this, EventArgs.Empty);
            GetRenderedTemplate(this, EventArgs.Empty);

            litAnnouncements.Text = Model.RenderedTemplate;

            var localTemplate = (BaseTemplate)Model.Template;
            if (!string.IsNullOrEmpty(localTemplate.JsFile) && File.Exists(Server.MapPath(localTemplate.JsFile)))
            {
                // Framework.jQuery.RegisterJQuery(Page);
                // Should not be required, jQuery is loaded by default

                    ClientResourceManager.RegisterScript(Page, localTemplate.JsFile);
            }
            if (!string.IsNullOrEmpty(localTemplate.CssFile) && File.Exists(Server.MapPath(localTemplate.CssFile)))
            {
                ClientResourceManager.RegisterStyleSheet(Page, localTemplate.CssFile);
            }

        }

        #region IActionable Implementation

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the modules custom Actions
        /// </summary>
        /// -----------------------------------------------------------------------------
        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actionCollection = new ModuleActionCollection
                {
                    {
                       
                        ModuleContext.GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, LocalResourceFile),
                        ModuleActionType.AddContent, "", "add.gif", ModuleContext.EditUrl(), false, SecurityAccessLevel.Edit, true, false
                    }
                };
                var moduleSecurity = new ModuleSecurity(ModuleContext.Configuration);
                if (moduleSecurity.HasEditTemplatePermission)
                {
                    actionCollection.Add(ModuleContext.GetNextActionID(), Localization.GetString("TemplateConfiguration.Action", LocalResourceFile), "Template", "", "icon_configuration_16px.png", ModuleContext.EditUrl("Template"), false, SecurityAccessLevel.View, true, false);
                }
                return actionCollection;
            }
        }

        #endregion

    }
}
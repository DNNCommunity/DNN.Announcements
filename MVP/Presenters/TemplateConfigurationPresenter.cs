using System;

using DotNetNuke.Common;
using DotNetNuke.Modules.Announcements.Components.Settings;
using DotNetNuke.Modules.Announcements.Components.Template;
using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Modules.Announcements.MVP.Views;
using DotNetNuke.Web.Mvp;

namespace DotNetNuke.Modules.Announcements.MVP.Presenters
{
    public class TemplateConfigurationPresenter : ModulePresenter<ITemplateConfiguration, TemplateConfigurationModel>
    {
        public TemplateConfigurationPresenter(ITemplateConfiguration view) : base(view)
        {
            View.GetSettings += GetSettings;
            View.GetTemplate += GetTemplate;


        }

        private void GetTemplate(object sender, EventArgs e)
        {
            Requires.NotNull("ModuleContext", ModuleContext);
            View.Model.Settings = new SettingsController().GetModuleSettings(ModuleContext.ModuleId, ModuleContext.TabModuleId);
        }

        private void GetSettings(object sender, EventArgs e)
        {
            Requires.NotNull("ModuleContext", ModuleContext);
            View.Model.Template = new TemplateController().GetTemplate(ModuleContext.ModuleId, ModuleContext.TabModuleId);
        }
    }
}
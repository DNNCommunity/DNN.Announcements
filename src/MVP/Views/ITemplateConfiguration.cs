using System;

using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Web.Mvp;

namespace DotNetNuke.Modules.Announcements.MVP.Views
{
    public interface ITemplateConfiguration : IModuleView<TemplateConfigurationModel>
    {
        event EventHandler GetSettings;
        event EventHandler GetTemplate;

    }
}
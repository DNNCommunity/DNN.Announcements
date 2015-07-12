using System.Collections.Generic;

using DotNetNuke.Modules.Announcements.Components.Business;

namespace DotNetNuke.Modules.Announcements.Components.Template
{
    public interface ITemplate
    {
        void LoadTemplate();

        void UpdateTemplate();

        string RenderTemplate(IEnumerable<AnnouncementInfo> announcements, bool editEnabled);
    }

}
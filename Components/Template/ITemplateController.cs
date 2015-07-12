using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.Modules.Announcements.Components.Template
{
    public interface ITemplateController
    {
        ITemplate GetTemplate(int moduleId, int tabModuleId);
    }
}
#region License

//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

#endregion

#region Usings

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

#endregion

namespace DotNetNuke.Modules.Announcements.Components.Common
{
    public class ModuleSecurity
    {
        private readonly bool _hasEditTemplatePermission;

        public ModuleSecurity(ModuleInfo moduleInfo)
        {
            var modulePermissionCollection = moduleInfo.ModulePermissions;
            _hasEditTemplatePermission = ModulePermissionController.HasModulePermission(modulePermissionCollection,
                                                                                   PermissionName.HasEditTemplatePermission);

        }

        public bool HasEditTemplatePermission
        {
            get { return _hasEditTemplatePermission; }
        }

        public static bool IsAdministrator()
        {
            var administratorRoleName = Globals.GetPortalSettings().AdministratorRoleName;
            return PortalSecurity.IsInRole(administratorRoleName);
        }
    }


}
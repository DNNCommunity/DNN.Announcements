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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Web.Api;

#endregion

namespace DotNetNuke.Modules.Announcements.Services
{
    public class AnnouncementsController : DnnApiController
    {
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [HttpGet]
        [ActionName("Current")]
        public HttpResponseMessage GetCurrentAnnouncements(string output)
        {
            try
            {

                IEnumerable<Components.Business.AnnouncementInfo> baseResults = 
                    new Components.Business.AnnouncementsController().GetCurrentAnnouncements(ActiveModule.ModuleID);
                List<AnnouncementInfo> results = 
                    baseResults.Select(announcementInfo => new AnnouncementInfo(announcementInfo)).ToList();
                return GenerateOutput(results, output);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [DnnAuthorize]
        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage GetAllAnnouncements(string output)
        {
            try
            {
                var mc = new ModuleController();
                var results = new List<AnnouncementInfo>();
                // get list of all announcements modules in the site
                var annModules = mc.GetModulesByDefinition(PortalSettings.PortalId, "Announcements");
                // loop through all the modules
                foreach (ModuleInfo m in annModules )
                {
                   // make sure to only include modules the user actually has access to
                   if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.View, "VIEW", m))
                   {
                       // get the current announcements of the module
                       IEnumerable<Components.Business.AnnouncementInfo> baseResults = new Components.Business.AnnouncementsController().GetCurrentAnnouncements(m.ModuleID);
                       // add to the total results list
                       results.AddRange(baseResults.Select(announcementInfo => new AnnouncementInfo(announcementInfo)).ToList());
                   }  
                }

                return GenerateOutput(results.OrderByDescending(a => a.PublishDate).ToList(), output);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage HelloWorld()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Hello World!");
        }

        /// <summary>
        /// This method returns all the announcements modules in the portal. 
        /// We don't want to show modules that the user has no access to, 
        /// so we are asking for DnnAuthorize here.
        /// Next, we filter the list of modules on those that the user has at least view permissions to
        /// </summary>
        /// <returns></returns>
        [DnnAuthorize]
        [HttpGet]
        public HttpResponseMessage Modules()
        {
            var mc = new ModuleController();
            var annModules = mc.GetModulesByDefinition(PortalSettings.PortalId, "Announcements");
            var results = (from ModuleInfo m in annModules where ModulePermissionController.HasModuleAccess(SecurityAccessLevel.View, "VIEW", m) select new AnnouncementsModule(m)).ToList();
            
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        private HttpResponseMessage GenerateOutput(List<AnnouncementInfo> results, string output)
        {
           
            //var output = System.Web.HttpContext.Current.Request.QueryString["OutputType"];
            switch (output)
            {
                case "xml":
                    return Request.CreateResponse(HttpStatusCode.OK, results, "application/xml");
                case "json":
                    return Request.CreateResponse(HttpStatusCode.OK, results, "application/json");
                case "rss":
                    return Request.CreateResponse(HttpStatusCode.OK, results, 
                        new SyndicationFeedFormatter(PortalSettings, System.Web.HttpContext.Current.Request), "application/rss+xml");
                case "atom":
                    return Request.CreateResponse(HttpStatusCode.OK, results, 
                        new SyndicationFeedFormatter(PortalSettings, System.Web.HttpContext.Current.Request), "application/atom+xml");
                default:
                    return Request.CreateResponse(HttpStatusCode.OK, results);
            }

        }

    }
}
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
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Modules.Announcements.Components.Template;
using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Modules.Announcements.MVP.Presenters;
using DotNetNuke.Modules.Announcements.MVP.Views;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvp;
using WebFormsMvp;

#endregion

namespace DotNetNuke.Modules.Announcements
{

	/// <summary>
	/// The Settings ModuleSettingsBase is used to manage the 
	/// settings for the Links Module
	/// </summary>
	/// <remarks>
	/// </remarks>
    [PresenterBinding(typeof(TemplateConfigurationPresenter))]
    public partial class TemplateConfiguration : ModuleView<TemplateConfigurationModel>, ITemplateConfiguration
	{
        public event EventHandler GetSettings;
        public event EventHandler GetTemplate;

        private void ApplySettings()
        {
            try
            {
                cmdLoadDefHeader.ToolTip = LocalizeString("LoadDefault.Help");
                cmdLoadDefItemTemplate.ToolTip = LocalizeString("LoadDefault.Help");
                cmdLoadDefAltItemTemplate.ToolTip = LocalizeString("LoadDefault.Help");
                cmdLoadDefSeparator.ToolTip = LocalizeString("LoadDefault.Help");
                cmdLoadDefFooterTemplate.ToolTip = LocalizeString("LoadDefault.Help");

                litTemplateHelp.Text = Localization.GetString("ModuleHelp.Text", LocalResourceFile);

                if (!Page.IsPostBack)
                {
                    var localTemplate = Model.Template as SimpleTemplate;

                    if (localTemplate != null)
                    {
                        txtTemplate.Text = localTemplate.ItemTemplate;
                        txtHeaderTemplate.Text = localTemplate.HeaderTemplate;
                        txtAltItemTemplate.Text = localTemplate.AltItemTemplate;
                        txtSeparator.Text = localTemplate.Separator;
                        txtFooterTemplate.Text = localTemplate.FooterTemplate;
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

		/// <summary>
		/// UpdateSettings saves the modified settings to the Database
		/// </summary>
		/// <remarks>
		/// </remarks>
        private void UpdateSettings()
        {
            try
            {
                var objModules = new ModuleController();

                ((SimpleTemplate)Model.Template).ItemTemplate = txtTemplate.Text.Trim();
                ((SimpleTemplate)Model.Template).HeaderTemplate = txtHeaderTemplate.Text;
                ((SimpleTemplate)Model.Template).AltItemTemplate = txtAltItemTemplate.Text;
                ((SimpleTemplate)Model.Template).Separator = txtSeparator.Text;
                ((SimpleTemplate)Model.Template).FooterTemplate = txtFooterTemplate.Text;

                ((SimpleTemplate)Model.Template).UpdateTemplate();

                //ModuleController.SynchronizeModule(MModuleId);
                //DataCache.RemoveCache(ModuleController.CacheKey(TabModuleId) + "_viewType");

                // redirect back to page
                Response.Redirect(Globals.NavigateURL(), true);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        //protected void CmdLoadDefHeaderClick(object sender, System.EventArgs e)
        //{
        //    Template.HeaderTemplate = Template.ItemTemplate;
        //    txtHeaderTemplate.Text = Template.HeaderTemplate;
        //}

        //protected void CmdLoadDefItemTemplateClick(object sender, System.EventArgs e)
        //{
        //    Template.ItemTemplate = Template.GetTemplate("Template", true);
        //    txtTemplate.Text = Template.ItemTemplate;
        //}

        //protected void CmdLoadDefAltItemTemplateClick(object sender, System.EventArgs e)
        //{
        //    Template.AltItemTemplate = Template.GetTemplate("AltItemTemplate", true);
        //    txtAltItemTemplate.Text = Template.AltItemTemplate;
        //}

        //protected void CmdLoadDefSeparatorClick(object sender, System.EventArgs e)
        //{
        //    Template.Separator = Template.GetTemplate("Separator", true);
        //    txtSeparator.Text = Template.Separator;
        //}

        //protected void CmdLoadDefFooterTemplateClick(object sender, System.EventArgs e)
        //{
        //    Template.FooterTemplate = Template.GetTemplate("FooterTemplate", true);
        //    txtFooterTemplate.Text = Template.FooterTemplate;
        //}

		 protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            //cmdLoadDefHeader.Click += CmdLoadDefHeaderClick;
            //cmdLoadDefItemTemplate.Click += CmdLoadDefItemTemplateClick;
            //cmdLoadDefAltItemTemplate.Click += CmdLoadDefAltItemTemplateClick;
            //cmdLoadDefSeparator.Click += CmdLoadDefSeparatorClick;
            //cmdLoadDefFooterTemplate.Click += CmdLoadDefFooterTemplateClick;
		     cmdUpdate.Click += CmdUpdateClick;
		}

        protected override void OnLoad(EventArgs e)
        {
            GetSettings(this, EventArgs.Empty);
            GetTemplate(this, EventArgs.Empty);
            ApplySettings();

            var moduleSecurity = new ModuleSecurity(ModuleContext.Configuration);
            if (!moduleSecurity.HasEditTemplatePermission)
                Response.Redirect(Globals.NavigateURL("Access Denied"));
            cancelHyperLink.NavigateUrl = Globals.NavigateURL();
        }

        protected void CmdUpdateClick(object sender, EventArgs e)
        {
            UpdateSettings();
        }

	}

}

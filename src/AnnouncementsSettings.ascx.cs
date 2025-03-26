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
using System.Web.UI.WebControls;

using DotNetNuke.Modules.Announcements.Components.Common;
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
    [PresenterBinding(typeof(AnnouncementsSettingsPresenter))]
    public partial class AnnouncementsSettings : SettingsView<AnnouncementsSettingsModel>, IAnnouncementsSettings
    {
        public event EventHandler GetSettings;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                GetSettings(this, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected override void OnSettingsLoaded()
        {
            base.OnSettingsLoaded();

            try
            {
                foreach (Enum i in Enum.GetValues(typeof(ViewTypes)))
                {
                    ddlViewType.Items.Add(new ListItem(Localization.GetString(Enum.GetName(typeof(ViewTypes), i), LocalResourceFile), Enum.GetName(typeof(ViewTypes), i)));
                }
                
                // This is not perfect, but the only way I found how to fix the settings reading issue, original code commented below.
                if (Model.ModuleSettings != null)
                {
                    if (Model.ModuleSettings.ContainsKey("history"))
                        txtHistory.Text = Model.ModuleSettings["history"].ToString();
                    if (Model.ModuleSettings.ContainsKey("descriptionLength"))
                        txtDescriptionLength.Text = Model.ModuleSettings["descriptionLength"].ToString();
                    if (Model.ModuleSettings.ContainsKey("editorHeight"))
                        txtEditorHeight.Text = Model.ModuleSettings["editorHeight"].ToString();
                    if (Model.ModuleSettings.ContainsKey("defaultViewType"))
                        ddlViewType.SelectedValue = Model.ModuleSettings["defaultViewType"].ToString();
                }
                //if (Model.Settings != null)
                //{
                //    txtHistory.Text = Model.Settings.History.ToDnnString();
                //    txtDescriptionLength.Text = Model.Settings.DescriptionLength.ToDnnString();
                //    txtEditorHeight.Text = Model.Settings.EditorHeight.ToDnnString();                    
                //    ddlViewType.SelectedValue = Utilities.ViewTypeToString(Model.Settings.DefaultViewType);
                //}
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected override void OnSavingSettings()
        {
            base.OnSavingSettings();
            try
            {

                Model.Settings.DefaultViewType = Utilities.StringToViewType(ddlViewType.SelectedValue);
                Model.Settings.History = txtHistory.Text.ToDnnInt();
                Model.Settings.DescriptionLength = txtDescriptionLength.Text.ToDnnInt();
                Model.Settings.EditorHeight = txtEditorHeight.Text.ToDnnInt();

                Model.Settings.Update();

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

    }

}

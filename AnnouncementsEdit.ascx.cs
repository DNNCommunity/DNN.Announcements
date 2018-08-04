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
using System.Data.SqlTypes;
using System.Globalization;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Modules.Announcements.MVP.Models;
using DotNetNuke.Modules.Announcements.MVP.Presenters;
using DotNetNuke.Modules.Announcements.MVP.Views;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Mvp;
using DotNetNuke.Web.UI.WebControls;

using WebFormsMvp;

#endregion

namespace DotNetNuke.Modules.Announcements
{

    /// <summary>
    /// The EditAnnouncements PortalModuleBase is used to manage Announcements
    /// </summary>
    /// <remarks>
    /// </remarks>
    [PresenterBinding(typeof(AnnouncementsEditPresenter))]
    public partial class AnnouncementsEdit : ModuleView<AnnouncementsEditModel>, IAnnouncementsEdit
    {

        public event EventHandler GetSettings;
        public event EventHandler<EditItemEventArgs> GetItem;
        public event EventHandler GetAnnouncement;
        public event EventHandler<EditItemEventArgs> DeleteAnnouncement;
        public event EventHandler<EditItemEventArgs> UpdateAnnouncement;

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            ClientResourceManager.RegisterStyleSheet(Page, Globals.ApplicationPath + "/DesktopModules/Announcements/AnnouncementsEdit.css", FileOrder.Css.ModuleCss);

            cmdDelete.Click += CmdDeleteClick;
            cmdUpdate.Click += CmdUpdateClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {

                GetSettings(this, EventArgs.Empty);
                GetItem(this, new EditItemEventArgs(Request.QueryString["ItemID"]));
                GetAnnouncement(this, new EditItemEventArgs(Model.ItemId));

                ApplySettings();
                if (Page.IsPostBack == false)
                {
                    BindForm();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the delete button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void CmdDeleteClick(object sender, EventArgs e)
        {
            try
            {
                DeleteAnnouncement(this, new EditItemEventArgs(Model.ItemId));
                Response.Redirect(ReturnURL, false);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }


        /// <summary>
        /// cmdUpdate_Click runs when the update button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void CmdUpdateClick(object sender, EventArgs e)
        {
            try
            {
                // verify data
                if (Page.IsValid)
                {
                    AnnouncementInfo announcement;
                    if (Model.IsAddMode)
                    {
                        // new item, create new announcement
                        announcement = new AnnouncementInfo
                            {
                                ItemID = Model.ItemId,
                                ModuleID = ModuleContext.ModuleId,
                                PortalID = ModuleContext.PortalId,
                                CreatedByUserID = ModuleContext.PortalSettings.UserId,
                                CreatedOnDate = DateTime.Now
                            };
                    }
                    else
                    {
                        // updating existing item, load it 
                        announcement = Model.AnnouncementInfo;
                    }

                    announcement.Title = txtTitle.Text;
                    announcement.ImageSource = urlImage.FilePath;
                    announcement.Description = teDescription.Text;
                    announcement.URL = ctlURL.Url;
                    announcement.PublishDate = GetDateTimeValue(publishDate, publishTime, DateTime.Now);
                    announcement.ExpireDate = GetDateTimeValue(expireDate, expireTime);
                    announcement.LastModifiedByUserID = ModuleContext.PortalSettings.UserId;
                    announcement.LastModifiedOnDate = DateTime.Now;
                    if (txtViewOrder.Text != "")
                    {
                        announcement.ViewOrder = Convert.ToInt32(txtViewOrder.Text);
                    }

                    UpdateAnnouncement(this, new EditItemEventArgs(announcement));

                    // url tracking
                    var objUrls = new UrlController();
                    objUrls.UpdateUrl(ModuleContext.PortalId, ctlURL.Url, ctlURL.UrlType, ctlURL.Log, ctlURL.Track, ModuleContext.ModuleId, ctlURL.NewWindow);

                    // redirect back to page
                    Response.Redirect(ReturnURL, true);

                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindForm()
        {
            if (!Model.IsAddMode)
            {
                if (Model.AnnouncementInfo != null)
                {
                    txtTitle.Text = Model.AnnouncementInfo.Title;
                    urlImage.FilePath = Model.AnnouncementInfo.ImageSource;
                    teDescription.Text = Model.AnnouncementInfo.Description;
                    ctlURL.Url = Model.AnnouncementInfo.URL;
                    if (!Null.IsNull(Model.AnnouncementInfo.ViewOrder))
                    {
                        txtViewOrder.Text = Convert.ToString(Model.AnnouncementInfo.ViewOrder);
                    }
                    if ((!Null.IsNull(Model.AnnouncementInfo.PublishDate)) &&
                        (Model.AnnouncementInfo.PublishDate != (DateTime)SqlDateTime.Null))
                    {
                        publishDate.SelectedDate = Model.AnnouncementInfo.PublishDate;
                        publishTime.SelectedDate = Model.AnnouncementInfo.PublishDate;
                    }
                    if ((!Null.IsNull(Model.AnnouncementInfo.ExpireDate)) &&
                        (Model.AnnouncementInfo.ExpireDate != (DateTime)SqlDateTime.Null))
                    {
                        expireDate.SelectedDate = Model.AnnouncementInfo.ExpireDate;
                        expireTime.SelectedDate = Model.AnnouncementInfo.ExpireDate;
                    }

                    ctlAudit.CreatedDate = Model.AnnouncementInfo.CreatedOnDate.ToString(CultureInfo.InvariantCulture);
                    ctlAudit.CreatedByUser = Model.AnnouncementInfo.CreatedByUserID.ToString(CultureInfo.InvariantCulture);
                    ctlAudit.LastModifiedByUser = Model.AnnouncementInfo.LastModifiedByUserID.ToString(CultureInfo.InvariantCulture);
                    ctlAudit.LastModifiedDate = Model.AnnouncementInfo.LastModifiedOnDate.ToString(CultureInfo.InvariantCulture);
                    ctlTracking.URL = Model.AnnouncementInfo.URL;
                    ctlTracking.ModuleID = ModuleContext.ModuleId;
                }
            }
        }

        private void ApplySettings()
        {
            teDescription.Height = Model.Settings.EditorHeight;
            cancelHyperLink.NavigateUrl = ReturnURL;

            urlImage.FileFilter = Globals.glbImageFileTypes;

            ctlURL.ShowLog = true;
            ctlURL.ShowNewWindow = true;
            ctlURL.ShowTrack = true;
            ctlURL.ShowUsers = true;

            cmdDelete.Visible = !Model.IsAddMode;
            ctlAudit.Visible = !Model.IsAddMode;
            ctlTracking.Visible = !Model.IsAddMode;


        }

        protected string ActiveDnnTab
        {
            get
            {
                var activeTab = Request.QueryString["activeTab"];
                if (!string.IsNullOrEmpty(activeTab))
                {
                    var tabControl = FindControl(activeTab);
                    if (tabControl != null)
                    {
                        return tabControl.ClientID;
                    }
                }

                return string.Empty;
            }
        }

        private string ReturnURL
        {
            get
            {
                var returnValue = UrlUtils.ValidReturnUrl(Request.Params["ReturnURL"]);
                if (string.IsNullOrEmpty(returnValue))
                    returnValue = Globals.NavigateURL();
                return returnValue;
            }
        }

        private DateTime? GetDateTimeValue(DnnDatePicker dnnDatePicker, DnnTimePicker dnnTimePicker)
        {
            DateTime? resultValue = null;

            if (dnnDatePicker.SelectedDate != null)
            {
                resultValue = dnnDatePicker.SelectedDate;
            }
            if ((dnnTimePicker.SelectedTime != null) && (resultValue.HasValue))
            {
                resultValue = resultValue.Value.Add((TimeSpan)dnnTimePicker.SelectedTime);
            }
            return resultValue;
        }

        private DateTime? GetDateTimeValue(DnnDatePicker dnnDatePicker, DnnTimePicker dnnTimePicker, DateTime defaultValue)
        {
            DateTime? resultValue = GetDateTimeValue(dnnDatePicker, dnnTimePicker);

            if (!resultValue.HasValue)
            {
                resultValue = defaultValue;
            }

            return resultValue;
        }


    }

}
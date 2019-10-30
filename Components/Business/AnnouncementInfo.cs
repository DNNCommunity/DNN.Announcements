#region License

//
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Announcements.Components.Common;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Tokens;

#endregion

namespace DotNetNuke.Modules.Announcements.Components.Business
{

    /// <summary>
    /// The AnnouncementInfo Class provides the Announcements Business Object
    /// </summary>
    [Serializable, XmlRoot("Announcement")]
    [TableName("Announcements")]
    [PrimaryKey("ItemID")]
    [Scope("ModuleID")]
    [Cacheable("Announcements", CacheItemPriority.Default, 20)]
    public class AnnouncementInfo : IPropertyAccess, IXmlSerializable
    {
        #region Constructors

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructs a new AnnouncementInfo instance
        /// </summary>
        /// <history>
        /// 	[erikvb]	11/19/2007  Described
        /// </history>
        /// -----------------------------------------------------------------------------
        public AnnouncementInfo()
        {
            CreatedOnDate = DateTime.Now;
            ExpireDate = null;
            PublishDate = null;
            CreatedByUserID = Null.NullInteger;
            LastModifiedByUserID = Null.NullInteger;
            LastModifiedOnDate = CreatedOnDate;
        }

        #endregion

        #region DB Fields

        public int ItemID { get; set; }
        public int ModuleID { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Description { get; set; }
        public int ViewOrder { get; set; }
        public DateTime? PublishDate { get; set; }
        public string ImageSource { get; set; }
        public int PortalID { get; set; }
        public int ContentItemID { get; set; }
        [IgnoreColumn]
        public bool IsEditable { get; set; }
        [Browsable(false), XmlIgnore]
        public int CreatedByUserID { get; set; }
        [Browsable(false), XmlIgnore]
        public DateTime CreatedOnDate { get; set; }
        [Browsable(false), XmlIgnore]
        public int LastModifiedByUserID { get; set; }
        [Browsable(false), XmlIgnore]
        public DateTime LastModifiedOnDate { get; set; }

        #endregion        
        
        #region NonDB Fields

        [IgnoreColumn]
        public UserInfo CreatedByUser(int portalId)
        {
            if (CreatedByUserID > Null.NullInteger)
            {
                UserInfo user = UserController.GetUserById(portalId, CreatedByUserID);
                return user;
            }
            return null;
        }

        [IgnoreColumn]
        public UserInfo LastModifiedByUser(int portalId)
        {
            if (LastModifiedByUserID > Null.NullInteger)
            {
                UserInfo user = UserController.GetUserById(portalId, LastModifiedByUserID);
                return user;
            }
            return null;
        }

        [IgnoreColumn]
        public String Permalink()
        {
            string cacheKey = string.Format("announcement_{0}_{1}", ModuleID, ItemID);
            return
                CBO.GetCachedObject<string>(
                    new CacheItemArgs(cacheKey,
                                      DataCache.ModuleCacheTimeOut,
                                      DataCache.ModuleControlsCachePriority,
                                      ModuleID,
                                      ItemID), PermalinkCallback);
        }

        private object PermalinkCallback(CacheItemArgs cacheItemArgs)
        {
            var moduleID = (int) cacheItemArgs.ParamList[0];
            var itemID = (int) cacheItemArgs.ParamList[1];

            var moduleInfo = new ModuleController().GetModule(moduleID);
            var url = Globals.NavigateURL(moduleInfo.TabID, "", string.Format("itemid={0}", itemID));
            return url;
        }

        private ContentItem _contentItem;
        [IgnoreColumn]
        public ContentItem ContentItem
        {
            get
            {
                if ((_contentItem == null) && ContentItemID != Null.NullInteger)
                {
                    _contentItem = new ContentController().GetContentItem(ContentItemID);
                }
                return _contentItem;
            }
        }

 

        private UrlTrackingInfo _urlTrackingInfo;
 
        [IgnoreColumn]
        private UrlTrackingInfo UrlTrackingInfo
        {
            get { return _urlTrackingInfo ?? (_urlTrackingInfo = new UrlController().GetUrlTracking(PortalID, URL, ModuleID)); }
        }

        private bool _trackClicks;

        [IgnoreColumn]
        public bool TrackClicks
        {
            get { return UrlTrackingInfo.TrackClicks; }
            set { _trackClicks = value; }
        }

        private bool _newWindow;

        [IgnoreColumn]
        public bool NewWindow
        {
            get { return UrlTrackingInfo != null && UrlTrackingInfo.NewWindow; }
            set { _newWindow = value; }
        }

        #endregion

        #region IXmlSerializable Implementation

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSchema returns the XmlSchema for this class
        /// </summary>
        /// <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
        /// <history>
        /// 	[cnurse]	08/17/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlSchema GetSchema()
        {
            return null;
        }


        private string readElement(XmlReader reader, string ElementName)
        {
            if ((reader.NodeType != XmlNodeType.Element) || reader.Name != ElementName)
            {
                reader.ReadToFollowing(ElementName);
            }
            return reader.NodeType == XmlNodeType.Element ? reader.ReadElementContentAsString() : "";
        }

        private bool CheckFileOrTab(string link)
        {
            return CheckFileOrTab(link, "");
        }

        private bool CheckFileOrTab(string link, string contentType)
        {
            int intId;
            PortalSettings portal = Globals.GetPortalSettings();
            if (link.StartsWith("FileID=", StringComparison.OrdinalIgnoreCase))
            {
                // the link is a file. Check wether it exists in the portal
                if (Int32.TryParse(link.Replace("FileID=", ""), out intId))
                {
                    var objFile = FileManager.Instance.GetFile(intId);
                    if (objFile != null)
                    {
                        if ((!(string.IsNullOrEmpty(contentType))) && (!(objFile.ContentType.StartsWith(contentType, StringComparison.OrdinalIgnoreCase))))
                        {
                            // the file exists but is of the wrong type
                            return false;
                        }
                    }
                    else
                    {
                        // the file does not exist for this portal
                        return false;
                    }
                }
            }
            else if (Int32.TryParse(link, out intId))
            {
                // the link is a tab
                var objTabController = new Entities.Tabs.TabController();
                if (objTabController.GetTab(intId, portal.PortalId, false) == null)
                {
                    // the tab does not exist
                    return false;
                }
            }

            // no reasons where found to reject the file
            return true;

        }

        /// <summary>
        /// ReadXml fills the object (de-serializes it) from the XmlReader passed
        /// </summary>
        /// <remarks></remarks>
        /// <param name="reader">The XmlReader that contains the xml for the object</param>
        public void ReadXml(XmlReader reader)
        {
            try
            {

                // when importing, ItemID will always be null.nullinteger (or -1)
                Title = readElement(reader, "Title");
                URL = readElement(reader, "URL");
                if (!(CheckFileOrTab(URL)))
                {
                    //  check whether the the fileid exists in the portal and is in fact an image.
                    URL = "";
                }
                int tempInt;
                ViewOrder = !(Int32.TryParse(readElement(reader, "ViewOrder"), out tempInt)) ? Null.NullInteger : tempInt;
                Description = readElement(reader, "Description");
                ImageSource = readElement(reader, "ImageSource");
                if (!(CheckFileOrTab(ImageSource, "image/")))
                {
                    //  check whether the fileid exists in the portal and is in fact an image.
                    ImageSource = "";
                }


                Boolean tempVar;
                bool.TryParse(readElement(reader, "TrackClicks"), out tempVar);
                TrackClicks = tempVar;
                Boolean tempVar2;
                bool.TryParse(readElement(reader, "NewWindow"), out tempVar2);
                NewWindow = tempVar2;

                DateTime tempDateTime;
                PublishDate = !(DateTime.TryParse(readElement(reader, "PublishDate"), out tempDateTime)) ? Null.NullDate : tempDateTime;
                ExpireDate = !(DateTime.TryParse(readElement(reader, "ExpireDate"), out tempDateTime)) ? DateTime.MaxValue : tempDateTime;

            }
            catch (Exception ex)
            {
                // log exception as DNN import routine does not do that
                Exceptions.LogException(ex);
                // re-raise exception to make sure import routine displays a visible error to the user
                throw new Exception("An error occured during import of an Announcement", ex);
            }

        }

        /// <summary>
        /// WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
        /// </summary>
        /// <remarks></remarks>
        /// <param name="writer">The XmlWriter that contains the xml for the object</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Announcement");
            writer.WriteElementString("ItemID", ItemID.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ModuleID", ModuleID.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Title", Title);
            writer.WriteElementString("URL", URL);
            writer.WriteElementString("ViewOrder", ViewOrder.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Description", Description);
            writer.WriteElementString("ImageSource", ImageSource);
            writer.WriteElementString("TrackClicks", TrackClicks.ToString());
            writer.WriteElementString("NewWindow", NewWindow.ToString());
            writer.WriteElementString("PublishDate", PublishDate.ToString());
            writer.WriteElementString("ExpireDate", ExpireDate.ToString());
            writer.WriteEndElement();
        }

        #endregion

        #region IPropertyAccess Implementation

        private string _localResourceFile = Globals.ApplicationPath + "/DesktopModules/Announcements/App_LocalResources/Announcements.ascx";


        public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            PortalSettings portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            string outputFormat = strFormat == string.Empty ? "D" : strFormat;
            switch (strPropertyName.ToLowerInvariant())
            {
                case "edit":
                    if (IsEditable)
                    {
                        string editUrl = Globals.NavigateURL("Edit",
                            $"mid={ModuleID.ToString(CultureInfo.InvariantCulture)}",
                            $"itemid={ItemID.ToString(CultureInfo.InvariantCulture)}");
                        if (portalSettings.EnablePopUps)
                        {
                            editUrl = UrlUtils.PopUpUrl(editUrl, null, portalSettings, false, false);
                        }
                        return "<a href=\"" + editUrl + "\"><img border=\"0\" src=\"" + Globals.ApplicationPath + "/icons/sigma/Edit_16X16_Standard_2.png\" alt=\"" + Localization.GetString("EditAnnouncement.Text", _localResourceFile) + "\" /></a>";
                    }
                    return string.Empty;
                case "itemid":
                    return (ItemID.ToString(outputFormat, formatProvider));
                case "moduleid":
                    return (ModuleID.ToString(outputFormat, formatProvider));
                case "title":
                    return PropertyAccess.FormatString(Title, strFormat);
                case "url":
                    return PropertyAccess.FormatString(string.IsNullOrEmpty(URL) ? URL : Utilities.FormatUrl(URL, portalSettings.ActiveTab.TabID, ModuleID, TrackClicks), strFormat);
                case "description":
                    return HttpUtility.HtmlDecode(Description);
                case "imagesource":
                case "rawimage":
                    string strValue = ImageSource;
                    if (strPropertyName.ToLowerInvariant() == "imagesource" && string.IsNullOrEmpty(strFormat))
                    {
                        strFormat = "<img src=\"{0}\" alt=\"" + Title + "\" />";
                    }

                    //Retrieve the path to the imagefile
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        //Get path from filesystem only when the image comes from within DNN.
                        // this is now legacy, from version 7.0.0, a real filename is saved in the DB
                        if (ImageSource != null && ImageSource.StartsWith("FileID="))
                        {

                            var objFile = FileManager.Instance.GetFile(Convert.ToInt32(strValue.Substring(7)));
                            if (objFile != null)
                            {
                                strValue = portalSettings.HomeDirectory + objFile.Folder + objFile.FileName;
                            }
                            else
                            {
                                strValue = "";
                            }
                        }
                        else
                        {
                            if (ImageSource != null && !ImageSource.ToLowerInvariant().StartsWith("http"))
                            {
                                strValue = portalSettings.HomeDirectory + ImageSource;
                            }
                        }
                        if (strValue != null)
                        {
                            strValue = PropertyAccess.FormatString(strValue, strFormat);
                        }
                    }
                    if (strValue == null)
                    {
                        return "";
                    }
                    else
                    {
                        return strValue;
                    }
                case "vieworder":
                    return (ViewOrder.ToString(outputFormat, formatProvider));
                case "createdbyuserid":
                    return (CreatedByUserID.ToString(outputFormat, formatProvider));
                case "createdbyuser":
                    UserInfo tmpUser = CreatedByUser(portalSettings.PortalId);
                    return tmpUser != null ? tmpUser.DisplayName : Localization.GetString("userUnknown.Text", _localResourceFile);
                case "lastmodifiedbyuserid":
                    return (LastModifiedByUserID.ToString(outputFormat, formatProvider));
                case "lastmodifiedbyuser":
                    UserInfo tmpUser2 = LastModifiedByUser(portalSettings.PortalId);
                    return tmpUser2 != null ? tmpUser2.DisplayName : Localization.GetString("userUnknown.Text", _localResourceFile);
                case "trackclicks":
                    return (PropertyAccess.Boolean2LocalizedYesNo(TrackClicks, formatProvider));
                case "newwindow":
                    return NewWindow ? "_blank" : "_self";
                case "createddate":
                case "createdondate":
                    return (CreatedOnDate.ToString(outputFormat, formatProvider));
                case "lastmodifiedondate":
                    return (LastModifiedOnDate.ToString(outputFormat, formatProvider));
                case "publishdate":
                    return PublishDate.HasValue ? (PublishDate.Value.ToString(outputFormat, formatProvider)) : "";
                case "expiredate":
                    return ExpireDate.HasValue ? (ExpireDate.Value.ToString(outputFormat, formatProvider)) : "";
                case "more":
                    return Localization.GetString("More.Text", _localResourceFile);
                case "readmore":
                    string strTarget = NewWindow ? "_new" : "_self";

                    return !(string.IsNullOrEmpty(URL))
                               ? "<a href=\"" + Utilities.FormatUrl(URL, portalSettings.ActiveTab.TabID, ModuleID, TrackClicks) +
                                 "\" target=\"" + strTarget + "\">" + Localization.GetString("More.Text", _localResourceFile) +
                                 "</a>"
                               : "";
                case "permalink":
                    return Permalink();
                case "subscribe":

                default:
                    PropertyNotFound = true;
                    break;
            }

            return Null.NullString;
        }

        [IgnoreColumn]
        public CacheLevel Cacheability
        {
            get
            {
                return CacheLevel.fullyCacheable;
            }
        }

        #endregion
    }
}


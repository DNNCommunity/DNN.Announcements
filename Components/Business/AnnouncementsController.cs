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
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Journal;
using DotNetNuke.Services.Search;
using DotNetNuke.Services.Search.Entities;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Services.Social.Notifications;
using Microsoft.ApplicationBlocks.Data;

#endregion

namespace DotNetNuke.Modules.Announcements.Components.Business
{

    /// <summary>
    /// The AnnouncementsController Class represents the Announcments Business Layer
    /// Methods in this class call methods in the Data Layer
    /// </summary>
    public class AnnouncementsController : ModuleSearchBase, IAnnouncementsController, IPortable, IUpgradeable
    {

        #region Public Methods

        public void AddAnnouncement(AnnouncementInfo announcement)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementInfo>();

                var objContentItem = new ContentItem
                {
                    Content = announcement.Description,
                    ContentTitle = announcement.Title,
                    Indexed = false,
                    ModuleID = announcement.ModuleID,
                    TabID = Null.NullInteger,
                    ContentKey = null,
                    ContentTypeId = 4
                };

                announcement.ContentItemID = new ContentController().AddContentItem(objContentItem);
                repository.Insert(announcement);
                }
        }

        public void UpdateAnnouncement(AnnouncementInfo announcement)
        {

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementInfo>();

                var objContentItem = new ContentItem
                {
                    ContentItemId = announcement.ContentItemID,
                    Content = announcement.Description,
                    ContentTitle = announcement.Title,
                    Indexed = false,
                    ModuleID = announcement.ModuleID,
                    TabID = Null.NullInteger,
                    ContentKey = null,
                    ContentTypeId = 4
                };

                if (objContentItem.ContentItemId == Null.NullInteger || objContentItem.ContentItemId == 0)
                {
                    announcement.ContentItemID = new ContentController().AddContentItem(objContentItem);
                }
                else
                {
                    new ContentController().UpdateContentItem(objContentItem);
                }

                repository.Update(announcement);

            }

        }


        public void DeleteAnnouncement(int moduleID, int itemID)
        {

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementInfo>();
                repository.Delete("WHERE ItemID = @0 AND ModuleId = @1", itemID, moduleID);
            }
        }

        public AnnouncementInfo GetAnnouncement(int itemID, int moduleID)
        {
            AnnouncementInfo announcement;
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementInfo>();
                announcement = repository.GetById(itemID, moduleID);
            }
            return announcement;
        }

        public IEnumerable<AnnouncementInfo> GetAnnouncements(int moduleId, DateTime startDate, DateTime endDate)
        {

            IEnumerable<AnnouncementInfo> announcements;

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementInfo>();
                announcements =
                    repository.Find(
                        "WHERE ModuleID = @0 AND ( ((PublishDate >= @1) OR @1 IS NULL) AND ((PublishDate <= @2) OR @2 IS NULL) )",
                        moduleId, Null.GetNull(startDate, DBNull.Value), Null.GetNull(endDate, DBNull.Value)).OrderBy(
                            a => a.ViewOrder).ThenByDescending(a => a.PublishDate);
            }
            return announcements;
        }

        /// <summary>
        /// Gets all the announcements for all modules.
        /// </summary>
        /// <returns>A list of all the announcements.</returns>
        internal IEnumerable<AnnouncementInfo> GetAnnouncements()
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementInfo>();
                return repository.Get();
            }
        }

        public IEnumerable<AnnouncementInfo> GetCurrentAnnouncements(int moduleId)
        {
            return GetCurrentAnnouncements(moduleId, Null.NullDate);
        }

        public IEnumerable<AnnouncementInfo> GetCurrentAnnouncements(int moduleId, DateTime startDate)
        {

            IEnumerable<AnnouncementInfo> announcements;

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementInfo>();
                var query = new StringBuilder();
                query.Append("WHERE ModuleID = @0 ")
                    .AppendLine("AND ( ((PublishDate >= @1) OR @1 IS NULL) AND (PublishDate <= @2) )")
                    .AppendLine("AND ( (ExpireDate > @2) OR (ExpireDate IS NULL) )");
                announcements = repository.Find(
                    query.ToString(),
                    moduleId,
                    Null.GetNull(startDate, DBNull.Value),
                    DateTime.UtcNow
                ).OrderBy(a => a.ViewOrder).ThenByDescending(a => a.PublishDate);
            }
            return announcements;
        }

        public IEnumerable<AnnouncementInfo> GetExpiredAnnouncements(int moduleId)
        {

            IEnumerable<AnnouncementInfo> announcements;

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementInfo>();
                announcements = repository.Find(
                    "WHERE ModuleID = @0 AND ExpireDate <= @1",
                    moduleId,
                    DateTime.UtcNow
                ).OrderBy(a => a.ViewOrder).ThenByDescending(a => a.PublishDate);
            }
            return announcements;
        }


        public void AddAnnouncementToJournal(AnnouncementInfo announcement, String journalType, ModuleInfo moduleInfo)
        {

            var objJournalType = JournalController.Instance.GetJournalType(journalType);

            var journalItem = new JournalItem
            {
                PortalId = announcement.PortalID,
                ProfileId = announcement.LastModifiedByUserID,
                UserId = announcement.LastModifiedByUserID,
                ContentItemId = announcement.ContentItemID,
                Title = announcement.Title
            };
            var data = new ItemData
            {
                Url = announcement.Permalink()
            };
            journalItem.ItemData = data;
            journalItem.Summary = HtmlUtils.Shorten(HtmlUtils.Clean(System.Web.HttpUtility.HtmlDecode(announcement.Description), false), 250, "...");
            journalItem.Body = announcement.Description;
            journalItem.JournalTypeId = objJournalType.JournalTypeId;
            journalItem.SecuritySet = "E,";

            JournalController.Instance.SaveJournalItem(journalItem, moduleInfo);

        }


        private void AddAnnouncementNotification(string subject, string body, UserInfo user, UserInfo sender, int portalId)
        {
            var notificationType = NotificationsController.Instance.GetNotificationType(Common.NotificationType.Announcements);
            var notification = new Notification { NotificationTypeID = notificationType.NotificationTypeId, Subject = subject, Body = body, IncludeDismissAction = true, SenderUserID = sender.UserID };
            NotificationsController.Instance.SendNotification(notification, portalId, null, new List<UserInfo> { user });
        }

        private void AddAnnouncementMessage(string subject, string body, UserInfo user,  UserInfo sender, int portalId)
        {
            var m = new Message { Subject = subject, Body = body };

            MessagingController.Instance.SendMessage(m, null, new List<UserInfo> { user }, null, sender);

        }

        public void SendNotifications(AnnouncementInfo announcement)
        {
            IEnumerable<AnnouncementsSubscriberInfo> subscribers = SubscriberController.GetSubscribers(announcement.ModuleID);
            foreach (var subscriber in subscribers)
            {
                var receiver = UserController.GetUserById(announcement.PortalID, subscriber.UserID);
                var sender = UserController.GetUserById(announcement.PortalID, announcement.LastModifiedByUserID);
                AddAnnouncementNotification(announcement.Title, announcement.Description, receiver, sender, announcement.PortalID);
            }
        }


        #endregion

        #region ModuleSearchBase Implementation

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDateUtc)
        {
            var moduleSettings = moduleInfo.ModuleSettings;
            int descriptionLenght = 100;
            if (!string.IsNullOrWhiteSpace(moduleSettings["descriptionLength"].ToString()))
            {
                int.TryParse(moduleSettings["descriptionLength"].ToString(), out descriptionLenght);
                if (descriptionLenght < 1) { descriptionLenght = 1950; }                    
                    //max length of description is 2000 char, take a bit less to make sure it fits...                
            }
            var searchDocuments = new List<SearchDocument>();
            var announcements = GetAnnouncements(moduleInfo.ModuleID, beginDateUtc, DateTime.MaxValue);
            foreach (var announcement in announcements)
            {
                var document = new SearchDocument();
                document.AuthorUserId = announcement.CreatedByUserID;
                document.Body = announcement.Description;
                document.CultureCode = moduleInfo.CultureCode;
                document.Description = HtmlUtils.Shorten(announcement.Description, descriptionLenght, "...");
                document.IsActive = CheckIfAnnouncementIsActive(announcement);
                document.ModifiedTimeUtc = announcement.LastModifiedOnDate.ToUniversalTime();
                document.ModuleDefId = moduleInfo.ModuleDefID;
                document.ModuleId = moduleInfo.ModuleID;
                document.PortalId = moduleInfo.PortalID;
                document.TabId = moduleInfo.TabID;
                document.Title = announcement.Title;
                document.UniqueKey = "Dnn_Announcements_" + announcement.ItemID.ToString(CultureInfo.InvariantCulture);
                searchDocuments.Add(document);
            }
            return searchDocuments;
        }

        private bool CheckIfAnnouncementIsActive(AnnouncementInfo announcement)
        {
            return (announcement.PublishDate < DateTime.UtcNow && (announcement.ExpireDate == null || announcement.ExpireDate > DateTime.UtcNow));
        }


        #endregion

        #region IUpgreadable implementation
        public string UpgradeModule(string Version)
        {
            try
            {
                switch (Version)
                {
                    case "07.02.04":
                        this.AdjustTimesToUTC();
                        break;
                    default:
                        break;
                }

                return "Success";
            }
            catch (Exception)
            {
                return "Announcements module upgrade failed.";
            }
        }

        #endregion

        #region IPortable Implementation

        /// <summary>
        /// ExportModule implements the IPortable ExportModule Interface using an XmlWriter
        /// and the IXmlSerializable Interface on the AnnoucnementInfo object
        /// </summary>
        /// <param name="ModuleID">The Id of the module to be exported</param>
        public string ExportModule(int ModuleID)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                    OmitXmlDeclaration = true
                };

            IEnumerable<AnnouncementInfo> arrAnnouncements = GetAnnouncements(ModuleID, Null.NullDate, Null.NullDate);
            var announcementInfos = arrAnnouncements as IList<AnnouncementInfo> ?? arrAnnouncements.ToList();
            if (announcementInfos.Count() != 0)
            {
                XmlWriter writer = XmlWriter.Create(sb, settings);

                //Write start of Annoucements Node
                writer.WriteStartElement("Announcements");

                foreach (AnnouncementInfo announcement in announcementInfos)
                {
                    announcement.WriteXml(writer);
                }

                //Write end of Annoucements Node
                writer.WriteEndElement();

                writer.Close();
            }

            return sb.ToString();


        }

        /// <summary>
        /// ImportModule implements the IPortable ImportModule Interface using an XmlReader
        /// and the IXmlSerializable Interface
        /// </summary>
        /// <param name="ModuleID">The Id of the module to be imported</param>
        public void ImportModule(int ModuleID, string Content, string Version, int UserId)
        {

            if (Version.StartsWith("03.04"))
            {
                // this is the legacy import function for version 03.04.00
                //INSTANT C# NOTE: Commented this declaration since looping variables in 'foreach' loops are declared in the 'foreach' header in C#:
                //				XmlNode xmlAnnouncement = null;
                XmlNode xmlAnnouncements = Globals.GetContent(Content, "Announcements");
                foreach (XmlNode xmlAnnouncement in xmlAnnouncements)
                {
                    AnnouncementInfo objAnnouncement = ImportAnnouncement(xmlAnnouncement);
                    if (objAnnouncement != null)
                    {
                        objAnnouncement.ModuleID = ModuleID;
                        objAnnouncement.CreatedByUserID = UserId;
                        objAnnouncement.CreatedOnDate = DateTime.UtcNow;

                        AddAnnouncement(objAnnouncement);

                    }
                }
            }
            else if (Version.StartsWith("03"))
            {
                // this is the legacy import function for all versions prior to version 03.04
                XmlNode xmlAnnouncements = Globals.GetContent(Content, "announcements");
                foreach (XmlNode xmlAnnouncement in xmlAnnouncements)
                {
                    var objAnnouncement = new AnnouncementInfo
                        {
                            ModuleID = ModuleID,
                            Title = XmlUtils.GetNodeValue(xmlAnnouncement, "title"),
                            URL = Globals.ImportUrl(ModuleID, XmlUtils.GetNodeValue(xmlAnnouncement, "url")),
                            Description = XmlUtils.GetNodeValue(xmlAnnouncement, "description"),
                            ViewOrder = XmlUtils.GetNodeValueInt(xmlAnnouncement, "vieworder"),
                            CreatedOnDate = XmlUtils.GetNodeValueDate(xmlAnnouncement, "createddate", DateTime.Now)
                        };
                    objAnnouncement.PublishDate = objAnnouncement.CreatedOnDate;
                    objAnnouncement.CreatedByUserID = UserId;
                    AddAnnouncement(objAnnouncement);
                }
            }
            else
            {
                // this is the current import function
                using (XmlReader reader = XmlReader.Create(new StringReader(Content)))
                {
                    if (reader.Read())
                    {
                        reader.ReadStartElement("Announcements");
                        if (reader.ReadState != ReadState.EndOfFile & reader.NodeType != XmlNodeType.None & reader.LocalName != "")
                        {
                            do
                            {
                                reader.ReadStartElement("Announcement");
                                var announcement = new AnnouncementInfo();

                                //Deserialize announcement
                                announcement.ReadXml(reader);

                                //initialize values of the new announcement to this module and this user
                                announcement.ItemID = Null.NullInteger;
                                announcement.ModuleID = ModuleID;
                                announcement.CreatedByUserID = UserId;
                                announcement.CreatedOnDate = DateTime.UtcNow;

                                //Save announcement
                                AddAnnouncement(announcement);
                            } while (reader.ReadToNextSibling("Announcement"));
                        }
                    }

                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Deserializes announcementInfo xml into new AnnouncementInfo instance.
        /// If an error occurs, Nothing is returned
        /// 
        /// deprecated
        /// </summary>
        /// <param name="xmlAnnouncement">the xml to be deserialized</param>
        /// <returns>AnnouncementInfo instance</returns>
        /// <remarks>
        /// </remarks>
        private AnnouncementInfo ImportAnnouncement(XmlNode xmlAnnouncement)
        {

            try
            {
                var xSer = new XmlSerializer(typeof(AnnouncementInfo));
                var objAnnouncement = (AnnouncementInfo)xSer.Deserialize(new StringReader(xmlAnnouncement.OuterXml));
                return objAnnouncement;

            }
            catch
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Prior to this version times where stored in local server timezone,
        /// this is wrong, times should always be stored UTC at the DataStore level
        /// and handled in code for timezone support depending on the needs.
        /// This upgrade ensures that existing data is updated to that new logic.
        /// </summary>
        private void AdjustTimesToUTC()
        {
            var announcements = this.GetAnnouncements();
            foreach (var announcement in announcements)
            {
                announcement.CreatedOnDate = TimeZoneInfo.ConvertTimeToUtc(announcement.CreatedOnDate);
                announcement.LastModifiedOnDate = TimeZoneInfo.ConvertTimeToUtc(announcement.LastModifiedOnDate);
                if (announcement.ExpireDate.HasValue)
                {
                    announcement.ExpireDate = TimeZoneInfo.ConvertTimeToUtc(announcement.ExpireDate.Value);
                }
                if (announcement.PublishDate.HasValue)
                {
                    announcement.PublishDate = TimeZoneInfo.ConvertTimeToUtc(announcement.PublishDate.Value);
                }
                this.UpdateAnnouncement(announcement);
            }

            var portals = PortalController.Instance.GetPortals();
            foreach (PortalInfo portal in portals)
            {
                var modules = ModuleController.Instance.GetModules(portal.PortalID);
                foreach (ModuleInfo module in modules)
                {
                    using (var context = DataContext.Instance())
                    {
                        context.Execute(
                            System.Data.CommandType.Text,
                            "UPDATE {databaseOwner}{objectQualifier}Modules SET LastContentModifiedOnDate = NULL WHERE ModuleID = @0",
                            module.ModuleID);
                    }
                }
            }
        }
    }

}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Modules.Announcements.Components.Business
{
    [Serializable, XmlRoot("AnnouncementsSubscriber")]
    [TableName("AnnouncementsSubscribers")]
    [PrimaryKey("AnnouncementsSubscriberID")]
    [Scope("ModuleID")]
    public class AnnouncementsSubscriberInfo
    {

        public AnnouncementsSubscriberInfo()
        {
            CreatedOnDate = DateTime.Now;
            CreatedByUserID = Null.NullInteger;
            LastModifiedByUserID = Null.NullInteger;
            LastModifiedOnDate = CreatedOnDate;
        }

        #region DBFields

        public int AnnouncementsSubscriberID { get; set; }
        public int ModuleID { get; set; }
        public int UserID { get; set; }

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
        #endregion


    }
}
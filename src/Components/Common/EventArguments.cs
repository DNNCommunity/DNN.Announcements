using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Modules.Announcements.Components.Business;

namespace DotNetNuke.Modules.Announcements.Components.Common
{
    public class EditItemEventArgs : EventArgs
    {
        public string ItemIdString { get; set; }
        public int ItemId { get; set; }
        public AnnouncementInfo Item { get; set; }

        public EditItemEventArgs(string itemId)
        {
            ItemIdString = itemId;
        }
        public EditItemEventArgs(int itemId)
        {
            ItemId = itemId;
        }
        public EditItemEventArgs(AnnouncementInfo item)
        {
            Item = item;
        }

    }
}
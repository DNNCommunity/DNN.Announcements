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
using System.Runtime.Serialization;

#endregion

namespace DotNetNuke.Modules.Announcements.Services
{

    [DataContract]
	public class AnnouncementInfo 
	{

#region Constructors

		/// <summary>
		/// Constructs a new AnnouncementInfo instance
		/// </summary>
		public AnnouncementInfo(Components.Business.AnnouncementInfo announcement)
		{
		    CreatedByUserID = announcement.CreatedByUserID;
		    CreatedOnDate = announcement.CreatedOnDate;
		    Description = announcement.Description;
		    ExpireDate = announcement.ExpireDate;
		    ImageSource = announcement.ImageSource;
		    ItemID = announcement.ItemID;
		    ModuleID = announcement.ModuleID;
		    PublishDate = announcement.PublishDate;
		    Title = announcement.Title;
		    URL = announcement.URL;
		    ViewOrder = announcement.ViewOrder;
		    LastModifiedByUserID = announcement.LastModifiedByUserID;
		    LastModifiedOnDate = announcement.LastModifiedOnDate;
		    PortalID = announcement.PortalID;
		    Permalink = announcement.Permalink();
		}

 

#endregion

#region Properties
        [DataMember]
	    public int ItemID { get; set; }
        [DataMember]
        public int ModuleID { get; set; }
        [DataMember]
	    public DateTime CreatedOnDate { get; set; }
        [DataMember]
	    public string Title { get; set; }
        [DataMember]
	    public string URL { get; set; }
        [DataMember]
	    public DateTime? ExpireDate { get; set; }
        [DataMember]
	    public string Description { get; set; }
        [DataMember]
	    public int ViewOrder { get; set; }
        [DataMember]
	    public int CreatedByUserID { get; set; }
        [DataMember]
	    public DateTime? PublishDate { get; set; }
        [DataMember]
	    public string ImageSource { get; set; }
        [DataMember]
        public int LastModifiedByUserID { get; set; }
        [DataMember]
        public DateTime LastModifiedOnDate { get; set; }
        [DataMember]
        public string Permalink { get; set; }
        [DataMember]
        public int PortalID { get; set; }

#endregion	

	}

}








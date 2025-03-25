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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using DotNetNuke.Entities.Portals;

#endregion

namespace DotNetNuke.Modules.Announcements.Services
{
    public class SyndicationFeedFormatter : MediaTypeFormatter
    {
        private readonly string atom = "application/atom+xml";
        private readonly string rss = "application/rss+xml";
        private PortalSettings portalSettings;
        private HttpRequest request;
        public SyndicationFeedFormatter(PortalSettings portalSettings, HttpRequest request)
        {
            this.portalSettings = portalSettings;
            this.request = request;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(atom));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(rss));
        }

        readonly Func<Type, bool> _supportedType = type => type == typeof(AnnouncementInfo) ||  type == typeof(List<AnnouncementInfo>);

        public override bool CanReadType(Type type)
        {
            return _supportedType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return _supportedType(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                if (_supportedType(type))
                    BuildSyndicationFeed(value, writeStream, content.Headers.ContentType.MediaType);
            });
        }

        private void BuildSyndicationFeed(object models, Stream stream, string contenttype)
        {
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority;

            var items = new List<SyndicationItem>();
            var feed = new SyndicationFeed
                {
                    Title = new TextSyndicationContent(portalSettings.PortalName),
                    Description = new TextSyndicationContent(portalSettings.Description),
                    Copyright = new TextSyndicationContent(portalSettings.FooterText),
                    Id = new Uri(baseUrl).ToString(),
                    LastUpdatedTime = new DateTimeOffset(DateTime.Now)
                };

            var link = new SyndicationLink(new Uri(baseUrl + request.RawUrl))
                {
                    RelationshipType = "self",
                    Title = portalSettings.PortalName
                };
            feed.Links.Add(link);

            link = new SyndicationLink(new Uri(baseUrl)) {MediaType = "text/html", Title = portalSettings.PortalName};
            feed.Links.Add(link);

            var list = models as List<AnnouncementInfo>;
            if (list != null)
            {
                items.AddRange(from announcementInfo in list select BuildSyndicationItem(announcementInfo));
            }
            else
            {
                items.Add(BuildSyndicationItem((AnnouncementInfo)models));
            }

            feed.Items = items;

            using (var writer = XmlWriter.Create(stream))
            {
                if (string.Equals(contenttype, atom))
                {
                    var atomformatter = new Atom10FeedFormatter(feed);
                    atomformatter.WriteTo(writer);
                }
                else
                {
                    var rssformatter = new Rss20FeedFormatter(feed);
                    rssformatter.WriteTo(writer);
                }
            }
        }

        private SyndicationItem BuildSyndicationItem(AnnouncementInfo ann)
        {
            var item = new SyndicationItem();
            if (ann.PublishDate != null)
                item.PublishDate = DateTime.SpecifyKind(ann.PublishDate.Value,DateTimeKind.Utc);

            item.Content = new TextSyndicationContent(ann.Description, TextSyndicationContentKind.Html);
            item.LastUpdatedTime = DateTime.SpecifyKind(ann.LastModifiedOnDate, DateTimeKind.Utc);
            item.Title = new TextSyndicationContent(ann.Title);
            var p = ann.Permalink;
            if (p != null)
            {
                //item.BaseUri = new Uri(p);
                item.AddPermalink(new Uri(p));
            }

            if (ann.URL != "")
            {
                item.Links.Add(new SyndicationLink(new Uri(ann.URL)));
                item.Id = ann.URL;
            }

            item.Authors.Add(new SyndicationPerson { Name = ann.LastModifiedByUserID.ToString(CultureInfo.InvariantCulture) });
            return item;
        }
    }
}
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

using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Modules.Announcements.Components.Business;
using DotNetNuke.Services.Cache;
using Shouldly;

namespace DotNetNuke.Announcements.Tests;

public class AnnouncementInfoTests
{
    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void WriteXml_WhenInfoHasUrl_Succeeds(bool trackClicks, bool newWindow)
    {
        var urlTrackingInfo = new UrlTrackingInfo { PortalID = 1, ModuleID = 2, Url = "3", TrackClicks = trackClicks, NewWindow = newWindow, };
        var info = new AnnouncementInfo { URL = urlTrackingInfo.Url, ModuleID = urlTrackingInfo.ModuleID, PortalID = urlTrackingInfo.PortalID, };
        var root = WriteXml(info, urlTrackingInfo);
        root.Element("TrackClicks").ShouldNotBeNull().Value.ShouldBe(trackClicks.ToString());
        root.Element("NewWindow").ShouldNotBeNull().Value.ShouldBe(newWindow.ToString());
    }

    [Fact]
    public void WriteXml_WhenInfoDoesNotHaveUrl_Succeeds()
    {
        var root = WriteXml(new AnnouncementInfo());
        root.Element("TrackClicks").ShouldNotBeNull().Value.ShouldBe(false.ToString());
        root.Element("NewWindow").ShouldNotBeNull().Value.ShouldBe(false.ToString());
    }

    private static XElement WriteXml(AnnouncementInfo info, params UrlTrackingInfo[] urlTrackingInfo)
    {
        ComponentFactory.RegisterComponentInstance<DataProvider>(new TestDataProvider(urlTrackingInfo));
        ((TestDataProvider)ComponentFactory.GetComponent<DataProvider>()).UrlTracking = urlTrackingInfo;
        ComponentFactory.RegisterComponentInstance<CachingProvider>(new TestCachingProvider());

        var sb = new StringBuilder();
        using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true, }))
        {
            info.WriteXml(writer);
        }

        return XElement.Parse(sb.ToString());
    }

    private class TestCachingProvider : CachingProvider
    {
        /// <inheritdoc />
        public override object? GetItem(string cacheKey)
        {
            return null;
        }
    }

    private class TestDataProvider(params UrlTrackingInfo[] infos) : DataProvider
    {
        public IEnumerable<UrlTrackingInfo> UrlTracking { get; set; } = infos;

        /// <inheritdoc />
        public override IDataReader GetUrlTracking(int PortalID, string Url, int ModuleID)
        {
            var dataTable = new DataTable
            {
                Columns = {
                    "UrlTrackingID",
                    "PortalID",
                    "Url",
                    "UrlType",
                    "Clicks",
                    "LastClick",
                    "CreatedDate",
                    "LogActivity",
                    "TrackClicks",
                    "ModuleId",
                    "NewWindow",
                },
            };
            foreach (var info in UrlTracking)
            {
                dataTable.LoadDataRow(
                    [
                        info.UrlTrackingID,
                        info.PortalID,
                        info.Url,
                        info.UrlType,
                        info.Clicks,
                        info.LastClick,
                        info.CreatedDate,
                        info.LogActivity,
                        info.TrackClicks,
                        info.ModuleID,
                        info.NewWindow,
                    ],
                    LoadOption.OverwriteChanges);
            }

            return new DataTableReader(dataTable);
        }

        /// <inheritdoc />
        public override void ExecuteNonQuery(string procedureName, params object[] commandParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void ExecuteNonQuery(int timeoutSec, string procedureName, params object[] commandParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void BulkInsert(string procedureName, string tableParameterName, DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void BulkInsert(string procedureName, string tableParameterName, DataTable dataTable, int timeoutSec)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void BulkInsert(string procedureName, string tableParameterName, DataTable dataTable, Dictionary<string, object> commandParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void BulkInsert(string procedureName, string tableParameterName, DataTable dataTable, int timeoutSec,
            Dictionary<string, object> commandParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IDataReader ExecuteReader(string procedureName, params object[] commandParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IDataReader ExecuteReader(int timeoutSec, string procedureName, params object[] commandParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override T ExecuteScalar<T>(string procedureName, params object[] commandParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override T ExecuteScalar<T>(int timeoutSec, string procedureName, params object[] commandParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IDataReader ExecuteSQL(string sql)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IDataReader ExecuteSQL(string sql, int timeoutSec)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string ExecuteScript(string script)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string ExecuteScript(string script, int timeoutSec)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string ExecuteScript(string connectionString, string sql)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string ExecuteScript(string connectionString, string sql, int timeoutSec)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IDataReader ExecuteSQLTemp(string connectionString, string sql)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IDataReader ExecuteSQLTemp(string connectionString, string sql, int timeoutSec)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IDataReader ExecuteSQLTemp(string connectionString, string sql, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IDataReader ExecuteSQLTemp(string connectionString, string sql, int timeoutSec, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool IsConnectionValid { get; }

        /// <inheritdoc />
        public override Dictionary<string, string> Settings { get; }
    }
}
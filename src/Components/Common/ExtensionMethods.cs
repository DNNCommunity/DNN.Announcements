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
using System.Globalization;
using DotNetNuke.Common.Utilities;

#endregion

namespace DotNetNuke.Modules.Announcements.Components.Common
{
    public static class HashtableExtensions
    {

        public static string GetString(this Hashtable valueTable, string valueName, string defaultValue)
        {
            string resultValue = defaultValue;
            if ((valueTable[valueName] != null))
            {
               resultValue = (string)valueTable[valueName];
            }
            return resultValue;
        }

        public static int GetInteger(this Hashtable valueTable, string valueName, int defaultValue)
        {
            int resultValue = defaultValue;
            if ((valueTable[valueName] != null))
            {
                resultValue = int.TryParse((string)valueTable[valueName], out resultValue) ? resultValue : Null.NullInteger;
            }

            if (resultValue == Null.NullInteger)
            {
                resultValue = defaultValue;
            }

            return resultValue;
        }

        public static bool GetBoolean(this Hashtable valueTable, string valueName, bool defaultValue)
        {
            bool resultValue = defaultValue;
            if ((valueTable[valueName] != null))
            {
                if (!bool.TryParse((string)valueTable[valueName],out resultValue))
                {
                    resultValue = defaultValue;
                }
            }
            return resultValue;
        }

        public static DateTime GetDateTime(this Hashtable valueTable, string valueName, DateTime defaultValue)
        {
            DateTime resultValue = defaultValue;
            if ((valueTable[valueName] != null))
            {
                if (!DateTime.TryParse((string)valueTable[valueName], CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out resultValue))
                {
                    resultValue = defaultValue;
                }
            }
            return resultValue;
        }

    }

    public static class StringExtensions
    {
        public static int ToDnnInt(this string s)
        {
            return ToDnnInt(s, Null.NullInteger);
        }

        public static int ToDnnInt(this string s, int defaultValue)
        {
            int resultValue = defaultValue;
            if (!String.IsNullOrEmpty(s))
            {
                if (!int.TryParse(s, out resultValue))
                {
                    resultValue = defaultValue;
                }
            }
            return resultValue;

        }
    }

    public static class IntExtensions
    {
        public static string ToDnnString(this int i)
        {
            return i == Null.NullInteger ? "" : i.ToString(CultureInfo.InvariantCulture);
        }

        public static bool IsDnnNull(this int i)
        {
            return i == Null.NullInteger;
        }
    }
}
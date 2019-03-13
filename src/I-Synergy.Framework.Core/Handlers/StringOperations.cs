﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ISynergy.Handlers
{
    public class StringWriterUTF8 : StringWriter
    {
        public StringWriterUTF8()
        {
        }

        public StringWriterUTF8(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }

    public class AlphanumericStringComparer : IComparer
    {
        private static List<string> GetList(string source)
        {
            var result = new List<string>();
            var stringItem = "";
            var flag = char.IsDigit(source[0]);

            foreach (var c in source.EnsureNotNull())
            {
                if (flag != char.IsDigit(c) || c == '\'')
                {
                    if (!string.IsNullOrEmpty(stringItem))
                        result.Add(stringItem);

                    stringItem = "";
                    flag = char.IsDigit(c);
                }
                if (char.IsDigit(c))
                {
                    stringItem += c;
                }
                if (char.IsLetter(c))
                {
                    stringItem += c;
                }
            }

            result.Add(stringItem);
            return result;
        }

        public int Compare(object x, object y)
        {
            var s1 = x as string;

            if (s1 is null)
            {
                return 0;
            }

            var s2 = y as string;

            if (s2 is null)
            {
                return 0;
            }

            if (s1 == s2)
            {
                return 0;
            }

            var len1 = s1.Length;
            var len2 = s2.Length;

            // Walk through two the strings with two markers.
            var str1 = GetList(s1);
            var str2 = GetList(s2);

            while (str1.Count != str2.Count)
            {
                if (str1.Count < str2.Count)
                {
                    str1.Add("");
                }
                else
                {
                    str2.Add("");
                }
            }

            var x1 = 0; var res = 0; var x2 = 0; var y2 = "";
            var status = false;
            var y1 = ""; var s1Status = false; var s2Status = false;

            var result = 0;

            for (var i = 0; i < str1.Count && i < str2.Count; i++)
            {
                status = int.TryParse(str1[i].ToString(), out res);
                if (res == 0)
                {
                    y1 = str1[i].ToString();
                    s1Status = false;
                }
                else
                {
                    x1 = Convert.ToInt32(str1[i].ToString());
                    s1Status = true;
                }

                status = int.TryParse(str2[i].ToString(), out res);
                if (res == 0)
                {
                    y2 = str2[i].ToString();
                    s2Status = false;
                }
                else
                {
                    x2 = Convert.ToInt32(str2[i].ToString());
                    s2Status = true;
                }

                //checking --the data comparision
                if (!s2Status && !s1Status)    //both are strings
                {
                    result = str1[i].CompareTo(str2[i]);
                }
                else if (s2Status && s1Status) //both are intergers
                {
                    if (x1 == x2)
                    {
                        if (str1[i].ToString().Length < str2[i].ToString().Length)
                        {
                            result = 1;
                        }
                        else if (str1[i].ToString().Length > str2[i].ToString().Length)
                            result = -1;
                        else
                            result = 0;
                    }
                    else
                    {
                        var st1ZeroCount = str1[i].ToString().Trim().Length - str1[i].ToString().TrimStart(new char[] { '0' }).Length;
                        var st2ZeroCount = str2[i].ToString().Trim().Length - str2[i].ToString().TrimStart(new char[] { '0' }).Length;
                        if (st1ZeroCount > st2ZeroCount)
                            result = -1;
                        else if (st1ZeroCount < st2ZeroCount)
                            result = 1;
                        else
                            result = x1.CompareTo(x2);
                    }
                }
                else
                {
                    result = str1[i].CompareTo(str2[i]);
                }
                if (result == 0)
                {
                    continue;
                }
                else
                    break;
            }
            return result;
        }
    }

    public static class StringOperations
    {
        public static string TruncateAt(string text, int maxWidth)
        {
            var result = text;

            if (text.Length > maxWidth)
            {
                result = text.Substring(0, maxWidth);
            }

            return result;
        }

        public static string ToCsv(params string[] items)
        {
            var sb = new StringBuilder(255);

            foreach (var i in items.EnsureNotNull())
            {
                sb.Append('"');
                sb.Append(i);
                sb.Append('"');
                sb.Append(',');
            }

            return sb.ToString().Chop();
        }

        public static string XElementToString(XElement xml)
        {
            var sw = new StringWriterUTF8(CultureInfo.CurrentCulture);
            var writer = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true, IndentChars = "\t", Encoding = Encoding.UTF8 });
            xml.WriteTo(writer);
            writer.Flush();
            writer.Dispose();
            return sw.ToString();
        }
    }
}
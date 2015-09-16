using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Dos.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class XLinqHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<XElement> AllElements(this XElement element, string name)
        {
            List<XElement> list = new List<XElement>();
            if (element != null)
            {
                list.AddRange(element.Nodes().Where(node => node.NodeType == XmlNodeType.Element).Select(node => (XElement) node).Where(item => item.Name.LocalName == name));
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<XElement> AllElements(this IEnumerable<XElement> elements, string name)
        {
            var list = new List<XElement>();
            foreach (XElement element in elements)
            {
                list.AddRange(element.AllElements(name));
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        public static XAttribute AnyAttribute(this XElement element, string name)
        {
            return element != null ? element.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == name) : null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static XElement AnyElement(this IEnumerable<XElement> elements, string name)
        {
            return elements.FirstOrDefault(element => element.Name.LocalName == name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static XElement AnyElement(this XElement element, string name)
        {
            return element != null ? (from node in element.Nodes() where node.NodeType == XmlNodeType.Element select (XElement) node).FirstOrDefault(element2 => element2.Name.LocalName == name) : null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static void AssertElementHasValue(this XElement element, string name)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            var element2 = element.AnyElement(name);
            if ((element2 == null) || string.IsNullOrEmpty(element2.Value))
            {
                throw new ArgumentNullException(name, string.Format("{0} is required", name));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static XElement FirstElement(this XElement element)
        {
            if (element.FirstNode.NodeType == XmlNodeType.Element)
            {
                return (XElement)element.FirstNode;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool GetBool(this XElement el, string name)
        {
            el.AssertElementHasValue(name);
            return (bool)el.GetElement(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool GetBoolOrDefault(this XElement el, string name)
        {
            return el.GetElementValueOrDefault<bool>(name, x => ((bool)x));
        }
        /// <summary>
        /// 
        /// </summary>
        public static DateTime GetDateTime(this XElement el, string name)
        {
            el.AssertElementHasValue(name);
            return (DateTime)el.GetElement(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static DateTime GetDateTimeOrDefault(this XElement el, string name)
        {
            return el.GetElementValueOrDefault<DateTime>(name, x => ((DateTime)x));
        }
        /// <summary>
        /// 
        /// </summary>
        public static decimal GetDecimal(this XElement el, string name)
        {
            el.AssertElementHasValue(name);
            return (decimal)el.GetElement(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static decimal GetDecimalOrDefault(this XElement el, string name)
        {
            return el.GetElementValueOrDefault<decimal>(name, x => ((decimal)x));
        }
        /// <summary>
        /// 
        /// </summary>
        public static XElement GetElement(this XElement element, string name)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return element.AnyElement(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static T GetElementValueOrDefault<T>(this XElement element, string name, Func<XElement, T> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            var element2 = element.GetElement(name);
            if ((element2 != null) && !string.IsNullOrEmpty(element2.Value))
            {
                return converter(element2);
            }
            return default(T);
        }
        /// <summary>
        /// 
        /// </summary>
        public static Guid GetGuid(this XElement el, string name)
        {
            el.AssertElementHasValue(name);
            return (Guid)el.GetElement(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static Guid GetGuidOrDefault(this XElement el, string name)
        {
            return el.GetElementValueOrDefault<Guid>(name, x => ((Guid)x));
        }
        /// <summary>
        /// 
        /// </summary>
        public static int GetInt(this XElement el, string name)
        {
            el.AssertElementHasValue(name);
            return (int)el.GetElement(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static int GetIntOrDefault(this XElement el, string name)
        {
            return el.GetElementValueOrDefault<int>(name, x => ((int)x));
        }
        /// <summary>
        /// 
        /// </summary>
        public static long GetLong(this XElement el, string name)
        {
            el.AssertElementHasValue(name);
            return (long)el.GetElement(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static long GetLongOrDefault(this XElement el, string name)
        {
            return el.GetElementValueOrDefault<long>(name, x => ((long)x));
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool? GetNullableBool(this XElement el, string name)
        {
            var element = el.GetElement(name);
            if ((element != null) && !string.IsNullOrEmpty(element.Value))
            {
                return (bool?)element;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static DateTime? GetNullableDateTime(this XElement el, string name)
        {
            var element = el.GetElement(name);
            if ((element != null) && !string.IsNullOrEmpty(element.Value))
            {
                return (DateTime?)element;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static decimal? GetNullableDecimal(this XElement el, string name)
        {
            var element = el.GetElement(name);
            if ((element != null) && !string.IsNullOrEmpty(element.Value))
            {
                return (decimal?)element;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static Guid? GetNullableGuid(this XElement el, string name)
        {
            var element = el.GetElement(name);
            if ((element != null) && !string.IsNullOrEmpty(element.Value))
            {
                return (Guid?)element;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static int? GetNullableInt(this XElement el, string name)
        {
            var element = el.GetElement(name);
            if ((element != null) && !string.IsNullOrEmpty(element.Value))
            {
                return (int?)element;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static long? GetNullableLong(this XElement el, string name)
        {
            var element = el.GetElement(name);
            if ((element != null) && !string.IsNullOrEmpty(element.Value))
            {
                return (long?)element;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan? GetNullableTimeSpan(this XElement el, string name)
        {
            var element = el.GetElement(name);
            if ((element != null) && !string.IsNullOrEmpty(element.Value))
            {
                return (TimeSpan?)element;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static string GetString(this XElement el, string name)
        {
            if (el == null)
            {
                return null;
            }
            return el.GetElementValueOrDefault<string>(name, x => x.Value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan GetTimeSpan(this XElement el, string name)
        {
            el.AssertElementHasValue(name);
            return (TimeSpan)el.GetElement(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static TimeSpan GetTimeSpanOrDefault(this XElement el, string name)
        {
            return el.GetElementValueOrDefault<TimeSpan>(name, x => ((TimeSpan)x));
        }
        /// <summary>
        /// 
        /// </summary>
        public static List<string> GetValues(this IEnumerable<XElement> els)
        {
            return els.Select(element => element.Value).ToList();
        }
    }
}

namespace NABSA
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// A collection of methods for searching
    /// through an XElement object
    /// </summary>
    static class Queries
    {
        /// <summary>
        /// Selects the first descendant with the given name
        /// (intended for finding descendants with unique names)
        /// </summary>
        /// <returns></returns>
        public static XElement FindFirst(XElement xml, string descendant)
        {
            var q =
                from el
                in xml.Descendants(descendant)
                select el;

            return q.First();            
        }

        /// <summary>
        /// Searches an XElement for a descendant which stores
        /// its name in a child element called 'Name'
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="name"></param>
        public static XElement FindByName(XElement xml, string name)
        {
            var q =
                from el 
                in xml.Descendants()
                where el.Elements().Any(e => e.Value == name)
                select el;

            return q.First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetElementNames(XElement xml)
        {
            var q =
                from el
                in xml.Elements()
                select el.Name.LocalName;

            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetElementValues(XElement xml)
        {
            var q =
                from el
                in xml.Elements()
                select el.Value;

            return q;
        }       
    }
}

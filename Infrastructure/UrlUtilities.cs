using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public static class UrlUtilities
    {
        public static string ToQuery(List<Tuple<string, string>> queryComponents, bool encodeKeys)
        {
            var escapedComponents = from comp in queryComponents
                                    select string.Format("{0}={1}",
                                        encodeKeys ? Uri.EscapeDataString(comp.Item1) : comp.Item1,
                                        Uri.EscapeDataString(comp.Item2));
            return string.Join("&", escapedComponents);
        }
    }
}

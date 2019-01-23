using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace Infrastructure
{
    public class FinnaApiSearch
    {
        private const string FilterKey = "filter[]";

        private int? limit;

        private Dictionary<string, string> otherFilters = new Dictionary<string, string>();

        public string BaseUrl { get; set; }
        public string LookFor { get; set; }
        public string Type { get; set; }
        public string Building { get; set; }
        public string PublishDate { get; set; }
        public string Genre { get; set; }

        public string Limit
        {
            get
            {
                if (limit.HasValue)
                {
                    return limit.Value.ToString(CultureInfo.InvariantCulture);
                }
                return null;
            }
            set
            {
                var newLimit = int.Parse(value, CultureInfo.InvariantCulture);
                if (newLimit < 0 || newLimit > 100)
                {
                    throw new ArgumentOutOfRangeException();
                }
                limit = newLimit;
            }
        }

        public List<string> Fields { get; set; }

        public void SetFilter(string field, string value)
        {
            otherFilters[field] = value;
        }

        public HttpRequestMessage CreateRequest()
        {
            var builder = new UriBuilder(BaseUrl);
            var queryComponents = GetQueryComponents();
            builder.Query = UrlUtilities.ToQuery(queryComponents, false);
            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            return request;
        }

        private List<Tuple<string, string>> GetQueryComponents()
        {
            var components = new List<Tuple<string, string>>();

            if (LookFor != null)
            {
                AddComponent(components, "lookfor", LookFor);
            }
            if (Type != null)
            {
                AddComponent(components, "type", Type);
            }

            if (Building != null)
            {
                AddComponent(components, FilterKey, Pair("building", Building));
            }
            if (PublishDate != null)
            {
                AddComponent(components, FilterKey, Pair("publishDate", PublishDate));
            }
            if (Genre != null)
            {
                AddComponent(components, FilterKey, Pair("genre", Genre));
            }

            AddOtherFilters(components);

            if (Limit != null)
            {
                AddComponent(components, "limit", Limit);
            }
            if (Fields != null)
            {
                foreach (string field in Fields)
                {
                    AddComponent(components, "field[]", field);
                }
            }

            AddStandardComponents(components);

            return components;
        }

        private void AddStandardComponents(List<Tuple<string, string>> components)
        {
            AddComponent(components, "type", "AllFields");
            AddComponent(components, "sort", "relevance,id asc");
            AddComponent(components, "page", "1");
            AddComponent(components, "prettyPrint", "false");
            AddComponent(components, "lng", "fi");
        }

        private void AddOtherFilters(List<Tuple<string, string>> components)
        {
            foreach (var field in otherFilters.Keys)
            {
                AddComponent(components, FilterKey, Pair(field, otherFilters[field]));
            }
        }

        private void AddComponent(List<Tuple<string, string>> components, string key, string value)
        {
            components.Add(new Tuple<string, string>(key, value));
        }

        private string Pair(string first, string second) => first + ":" + second;
    }
}
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;

namespace Infrastructure
{
    public class GoodreadsApiBookSearch
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string Query { get; set; }

        public HttpRequestMessage CreateRequest()
        {
            var builder = new UriBuilder(BaseUrl);
            var queryComponents = GetQueryComponents();
            builder.Query = UrlUtilities.ToQuery(queryComponents, false);
            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            return request;
        }

        public GoodreadsBookSearchResult ParseResponse(HttpResponseMessage response)
        {
            var noMatch = new GoodreadsBookSearchResult()
            {
                NumResults = 0,
                BestMatch = null
            };
            if (!response.IsSuccessStatusCode)
            {
                return noMatch;
            }

            var body = response.Content.ReadAsStringAsync().Result;
            var xmlRoot = XElement.Parse(body);

            var totalResults = int.Parse((string)xmlRoot.Descendants("total-results").First());

            var resultsElem = xmlRoot.Descendants("results").First();
            if (resultsElem.Elements().Count() == 0)
            {
                return noMatch;
            }

            var bookElem = resultsElem.Element("work").Element("best_book");
            var match = new GoodreadsBestMatch()
            {
                BookID = int.Parse((string)bookElem.Element("id")),
                Author = (string)bookElem.Element("author").Element("name"),
                Title = (string)bookElem.Element("title"),
                IsReliableMatch = true
            };
            return new GoodreadsBookSearchResult()
            {
                NumResults = totalResults,
                BestMatch = match
            };
        }

        private List<Tuple<string, string>> GetQueryComponents()
        {
            if (string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(Query))
            {
                throw new ArgumentException();
            }

            var components = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("key", ApiKey),
                new Tuple<string, string>("q", Query)
            };
            return components;
        }
    }
}

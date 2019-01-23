using Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;

namespace Infrastructure
{
    public class GoodreadsApiReviews
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public int BookID { get; set; } = -1;

        public HttpRequestMessage CreateRequest()
        {
            var builder = new UriBuilder(BaseUrl);
            var queryComponents = GetQueryComponents();
            builder.Query = UrlUtilities.ToQuery(queryComponents, false);
            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            return request;
        }

        public GoodreadsReviewsResult ParseResponse(HttpResponseMessage response)
        {
            var result = new GoodreadsReviewsResult();
            if (!response.IsSuccessStatusCode)
            {
                return result;
            }

            var body = response.Content.ReadAsStringAsync().Result;
            var xmlRoot = XElement.Parse(body);

            var ratingStr = (string)xmlRoot.Descendants("average_rating").First();
            var rating = float.Parse(ratingStr, CultureInfo.InvariantCulture);
            var reviews = (string)xmlRoot.Descendants("reviews_widget").First();

            result.Reviews = new GoodreadsReviews()
            {
                AverageRating = rating,
                ReviewsHtml = reviews
            };
            return result;
        }

        private List<Tuple<string, string>> GetQueryComponents()
        {
            if (string.IsNullOrEmpty(ApiKey) || BookID < 0)
            {
                throw new ArgumentException();
            }

            var components = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("key", ApiKey),
                new Tuple<string, string>("id", BookID.ToString())
            };
            return components;
        }
    }
}

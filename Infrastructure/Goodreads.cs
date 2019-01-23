using Core;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Goodreads : IGoodreads
    {
        public async Task<GoodreadsBookSearchResult> SearchBooksAsync(Item item)
        {
            var settings = ConfigurationManager.AppSettings;
            var baseUrl = settings["GoodreadsSearchBaseUrl"];
            var apiKey = settings["GoodreadsApiKey"];

            var search = new GoodreadsApiBookSearch()
            {
                BaseUrl = baseUrl,
                ApiKey = apiKey,
                Query = item.Title + " " + item.AuthorName
            };
            HttpRequestMessage request = search.CreateRequest();

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                GoodreadsBookSearchResult result = search.ParseResponse(response);
                return result;
            }
        }

        public async Task<GoodreadsReviewsResult> GetReviewsAsync(int bookID)
        {
            var settings = ConfigurationManager.AppSettings;
            var baseUrl = settings["GoodreadsReviewsBaseUrl"];
            var apiKey = settings["GoodreadsApiKey"];

            var link = new GoodreadsApiReviews()
            {
                BaseUrl = baseUrl,
                ApiKey = apiKey,
                BookID = bookID
            };
            HttpRequestMessage request = link.CreateRequest();

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                GoodreadsReviewsResult result = link.ParseResponse(response);
                return result;
            }
        }
    }
}

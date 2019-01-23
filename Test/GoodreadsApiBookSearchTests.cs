using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure;
using System.Net.Http;

namespace Test
{
    [TestClass()]
    public class GoodreadsApiBookSearchTests
    {
        const string SearchResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<GoodreadsResponse>
  <Request>
    <authentication>true</authentication>
      <key><![CDATA[2Keb27mmSYFsa2ZX4U49A]]></key>
    <method><![CDATA[search_index]]></method>
  </Request>
  <search>
  <query><![CDATA[Ender's Game Orson Scott Card]]></query>
    <results-start>1</results-start>
    <results-end>20</results-end>
    <total-results>2</total-results>
    <source>Goodreads</source>
    <query-time-seconds>0.20</query-time-seconds>
    <results>
        <work>
  <id type=""integer"">2422333</id>
  <books_count type=""integer"">15</books_count>
  <ratings_count type=""integer"">983427</ratings_count>
  <text_reviews_count type=""integer"">40580</text_reviews_count>
  <original_publication_year type=""integer"">1985</original_publication_year>
  <original_publication_month type=""integer"" nil=""true""/>
  <original_publication_day type=""integer"" nil=""true""/>
  <average_rating>4.30</average_rating>
  <best_book type=""Book"">
    <id type=""integer"">375802</id>
    <title>Ender's Game (Ender's Saga, #1)</title>
    <author>
      <id type=""integer"">589</id>
      <name>Orson Scott Card</name>
    </author>
    <image_url>https://images.gr-assets.com/books/1408303130m/375802.jpg</image_url>
    <small_image_url>https://images.gr-assets.com/books/1408303130s/375802.jpg</small_image_url>
  </best_book>
</work>

        <work>
  <id type=""integer"">27117348</id>
  <books_count type=""integer"">1</books_count>
  <ratings_count type=""integer"">74</ratings_count>
  <text_reviews_count type=""integer"">7</text_reviews_count>
  <original_publication_year type=""integer"">2013</original_publication_year>
  <original_publication_month type=""integer"">6</original_publication_month>
  <original_publication_day type=""integer"">14</original_publication_day>
  <average_rating>4.39</average_rating>
  <best_book type=""Book"">
    <id type=""integer"">19089701</id>
    <title>Ender's Game by Orson Scott Card (Expert Book Review)</title>
    <author>
      <id type=""integer"">7337707</id>
      <name>Brainy Book Reviews</name>
    </author>
    <image_url>https://s.gr-assets.com/assets/nophoto/book/111x148-bcc042a9c91a29c1d680899eff700a03.png</image_url>
    <small_image_url>https://s.gr-assets.com/assets/nophoto/book/50x75-a91bf249278a81aabab721ef782c4a74.png</small_image_url>
  </best_book>
</work>

    </results>
</search>

</GoodreadsResponse>
";

        [TestMethod()]
        public void CreateRequestTest()
        {
            var search = new GoodreadsApiBookSearch()
            {
                ApiKey = "123",
                BaseUrl = "https://abc.invalid/search.xml",
                Query = "x y z"
            };

            var request = search.CreateRequest();

            var uri = request.RequestUri;
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("https", uri.Scheme);
            Assert.AreEqual("abc.invalid", uri.Host);
            Assert.AreEqual("/search.xml", uri.AbsolutePath);
            var query = request.RequestUri.Query;
            Assert.IsTrue(query.Contains("q=x%20y%20z"));
            Assert.IsTrue(query.Contains("key=123"));
        }

        [TestMethod()]
        public void ParseResponseTest()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new StringContent(SearchResponse);
            var search = new GoodreadsApiBookSearch();

            var result = search.ParseResponse(response);

            Assert.AreEqual(2, result.NumResults);
            Assert.AreEqual(375802, result.BestMatch.BookID);
            Assert.AreEqual("Orson Scott Card", result.BestMatch.Author);
            Assert.AreEqual("Ender's Game (Ender's Saga, #1)", result.BestMatch.Title);
        }
    }
}
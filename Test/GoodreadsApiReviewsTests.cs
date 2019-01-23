using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure;
using System.Net.Http;

namespace Test
{
    [TestClass()]
    public class GoodreadsApiReviewsTests
    {
        const string ReviewsResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<GoodreadsResponse>
  <Request>
    <authentication>true</authentication>
      <key><![CDATA[2Keb27mmSYFsa2ZX4U49A]]></key>
    <method><![CDATA[book_show]]></method>
  </Request>
  <book>
  <id>375802</id>
  <title><![CDATA[Ender's Game (Ender's Saga, #1)]]></title>
  <isbn><![CDATA[0812550706]]></isbn>
  <isbn13><![CDATA[9780812550702]]></isbn13>
  <asin><![CDATA[]]></asin>
  <kindle_asin><![CDATA[]]></kindle_asin>
  <marketplace_id><![CDATA[]]></marketplace_id>
  <country_code><![CDATA[FI]]></country_code>
  <image_url>https://images.gr-assets.com/books/1408303130m/375802.jpg</image_url>
  <small_image_url>https://images.gr-assets.com/books/1408303130s/375802.jpg</small_image_url>
  <publication_year>1994</publication_year>
  <publication_month>7</publication_month>
  <publication_day></publication_day>
  <publisher>TOR</publisher>
  <language_code>eng</language_code>
  <is_ebook>false</is_ebook>
  <description><![CDATA[Andrew ""Ender"" Wiggin...]]></description>
  <work>
  <id type=""integer"">2422333</id>
  <books_count type=""integer"">238</books_count>
  <best_book_id type=""integer"">375802</best_book_id>
  <reviews_count type=""integer"">1406461</reviews_count>
  <ratings_sum type=""integer"">4230246</ratings_sum>
  <ratings_count type=""integer"">983483</ratings_count>
  <text_reviews_count type=""integer"">40585</text_reviews_count>
  <original_publication_year type=""integer"">1985</original_publication_year>
  <original_publication_month type=""integer"" nil=""true""/>
  <original_publication_day type=""integer"" nil=""true""/>
  <original_title>Ender's Game</original_title>
  <original_language_id type=""integer"" nil=""true""/>
  <media_type>book</media_type>
  <rating_dist>5:523578|4:296979|3:115452|2:30610|1:16864|total:983483</rating_dist>
  <desc_user_id type=""integer"">4628840</desc_user_id>
  <default_chaptering_book_id type=""integer"">820750</default_chaptering_book_id>
  <default_description_language_code nil=""true""/>
</work>
  <average_rating>4.30</average_rating>
  <num_pages><![CDATA[324]]></num_pages>
  <format><![CDATA[Mass Market Paperback]]></format>
  <edition_information><![CDATA[Author's Definitive Edition, Revised mass market edition  (US / CAN)]]></edition_information>
  <ratings_count><![CDATA[912501]]></ratings_count>
  <text_reviews_count><![CDATA[35135]]></text_reviews_count>
  <url><![CDATA[https://www.goodreads.com/book/show/375802.Ender_s_Game]]></url>
  <link><![CDATA[https://www.goodreads.com/book/show/375802.Ender_s_Game]]></link>
  <authors>
<author>
<id>589</id>
<name>Orson Scott Card</name>
<role></role>
<image_url nophoto='false'>
<![CDATA[https://images.gr-assets.com/authors/1294099952p5/589.jpg]]>
</image_url>
<small_image_url nophoto='false'>
<![CDATA[https://images.gr-assets.com/authors/1294099952p2/589.jpg]]>
</small_image_url>
<link><![CDATA[https://www.goodreads.com/author/show/589.Orson_Scott_Card]]></link>
<average_rating>4.08</average_rating>
<ratings_count>2380677</ratings_count>
<text_reviews_count>99244</text_reviews_count>
</author>
</authors>

    <reviews_widget>
      <![CDATA[
        <style>
  #goodreads-widget {
    font-family: georgia, serif;
    padding: 18px 0;
    width:565px;
  }
  #goodreads-widget h1 {
    font-weight:normal;
    font-size: 16px;
    border-bottom: 1px solid #BBB596;
    margin-bottom: 0;
  }
  #goodreads-widget a {
    text-decoration: none;
    color:#660;
  }
  iframe{
    background-color: #fff;
  }
  #goodreads-widget a:hover { text-decoration: underline; }
  #goodreads-widget a:active {
    color:#660;
  }
  #gr_footer {
    width: 100%;
    border-top: 1px solid #BBB596;
    text-align: right;
  }
  #goodreads-widget .gr_branding{
    color: #382110;
    font-size: 11px;
    text-decoration: none;
    font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;
  }
</style>
<div id=""goodreads-widget"">
  <div id=""gr_header""><h1><a rel=""nofollow"" href=""https://www.goodreads.com/book/show/375802.Ender_s_Game"">Ender&#39;s Game Reviews</a></h1></div>
  <iframe id=""the_iframe"" src=""https://www.goodreads.com/api/reviews_widget_iframe?did=DEVELOPER_ID&amp;format=html&amp;isbn=0812550706&amp;links=660&amp;min_rating=&amp;review_back=fff&amp;stars=000&amp;text=000"" width=""565"" height=""400"" frameborder=""0""></iframe>
  <div id=""gr_footer"">
    <a class=""gr_branding"" target=""_blank"" rel=""nofollow noopener noreferrer"" href=""https://www.goodreads.com/book/show/375802.Ender_s_Game?utm_medium=api&amp;utm_source=reviews_widget"">Reviews from Goodreads.com</a>
  </div>
</div>

      ]]>
    </reviews_widget>
</book>
</GoodreadsResponse>
";

        [TestMethod()]
        public void CreateRequestTest()
        {
            var search = new GoodreadsApiReviews()
            {
                ApiKey = "123",
                BaseUrl = "https://abc.invalid/show.xml",
                BookID = 1001
            };

            var request = search.CreateRequest();

            var uri = request.RequestUri;
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("https", uri.Scheme);
            Assert.AreEqual("abc.invalid", uri.Host);
            Assert.AreEqual("/show.xml", uri.AbsolutePath);
            var query = request.RequestUri.Query;
            Assert.IsTrue(query.Contains("id=1001"));
            Assert.IsTrue(query.Contains("key=123"));
        }

        [TestMethod()]
        public void ParseResponseTest()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new StringContent(ReviewsResponse);
            var link = new GoodreadsApiReviews();

            var result = link.ParseResponse(response);

            Assert.AreEqual(4.3, result.Reviews.AverageRating, 0.001);
            Assert.IsTrue(result.Reviews.ReviewsHtml.Contains("iframe"));
        }
    }
}
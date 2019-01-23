using System.Threading.Tasks;

namespace Core
{
    public interface IGoodreads
    {
        Task<GoodreadsBookSearchResult> SearchBooksAsync(Item item);
        Task<GoodreadsReviewsResult> GetReviewsAsync(int bookID);
    }

    public class GoodreadsBookSearchResult
    {
        public int NumResults { get; set; }
        public GoodreadsBestMatch BestMatch { get; set; }
    }

    public class GoodreadsBestMatch
    {
        public int BookID { get; set; }
        public bool IsReliableMatch { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
    }

    public class GoodreadsReviewsResult
    {
        public GoodreadsReviews Reviews { get; set; }
    }

    public class GoodreadsReviews
    {
        public float AverageRating { get; set; }
        public string ReviewsHtml { get; set; }
    }
}

const server = 'http://localhost:59031';

export default {
  RECOMMENDATIONS_ENDPOINT: server + '/api/Recommendations',
  searchReviewsUrl: (id) => `${server}/api/Items/SearchReviews?id=${id}`,
  getReviewsUrl: (grBookID) => `${server}/api/Items/Reviews?gr_book_id=${grBookID}`,
  THUMBNAILS_BASE_URL: server + '/Content/Thumbnails/',
  FINNA_BASE_URL: 'https://heili.finna.fi',
  DELAY_MILLIS: 1500
};
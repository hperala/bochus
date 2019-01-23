using Core;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace Web.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class ItemsController : ApiController
    {
        private IRepository repository;
        private IGoodreads goodreads;
        private IThrottler goodreadsAccess;

        public ItemsController(
            IRepository repository, 
            IGoodreads goodreads, 
            IThrottler goodreadsAccess)
        {
            this.repository = repository;
            this.goodreads = goodreads;
            this.goodreadsAccess = goodreadsAccess;
        }

        // GET: api/Items/5
        [ResponseType(typeof(Item))]
        public IHttpActionResult GetItem(int id)
        {
            Item item = repository.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        [Route("api/Items/Import")]
        [ResponseType(typeof(Item))]
        public IHttpActionResult ImportFinnaItem([FromBody]JObject finnaItem)
        {
            var settings = WebConfigurationManager.AppSettings;
            var separator = settings["SubjectPartSeparator"];
            var importer = new FinnaImporter(repository, separator);

            Item item = importer.CreateItem(finnaItem);
            repository.AddItem(item);
            CreateAndSaveTitleCard(item);

            repository.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { controller = "Items", id = item.ID }, item);
        }

        [HttpGet]
        [Route("api/Items/SearchReviews")]
        [ResponseType(typeof(GoodreadsBookSearchResult))]
        public async Task<IHttpActionResult> SearchReviewsAsync(int id)
        {
            if (!goodreadsAccess.IsAvailable(DateTime.Now))
            {
                return StatusCode(HttpStatusCode.ServiceUnavailable);
            }

            try
            {
                Item item = repository.GetItem(id);
                if (item == null)
                {
                    return NotFound();
                }

                var result = await goodreads.SearchBooksAsync(item);
                return Ok(result);
            }
            finally
            {
                goodreadsAccess.RequestCompleted(DateTime.Now);
            }
        }

        [HttpGet]
        [Route("api/Items/Reviews")]
        [ResponseType(typeof(GoodreadsBookSearchResult))]
        public async Task<IHttpActionResult> GetReviewsAsync(int gr_book_id)
        {
            if (!goodreadsAccess.IsAvailable(DateTime.Now))
            {
                return StatusCode(HttpStatusCode.ServiceUnavailable);
            }

            try
            {
                var result = await goodreads.GetReviewsAsync(gr_book_id);
                return Ok(result);
            }
            finally
            {
                goodreadsAccess.RequestCompleted(DateTime.Now);
            }
        }

        private void CreateAndSaveTitleCard(Item item)
        {
            var settings = WebConfigurationManager.AppSettings;
            int w = int.Parse(settings["ThumbnailWidth"]);
            int h = int.Parse(settings["ThumbnailHeight"]);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(w, h);
            var initial = item.Title.Length > 0 ? item.Title[0].ToString() : "";

            var generator = new TitleCard(
                graphics: System.Drawing.Graphics.FromImage(bitmap),
                strings: new string[] { item.AuthorName, item.Title },
                decorativeInitial: initial,
                stylingKeyString: item.AuthorLastName,
                width: w,
                height: h);
            generator.DrawBackground();
            generator.DrawForeground();

            var directory = HostingEnvironment.MapPath("~/Content/Thumbnails");
            System.IO.Directory.CreateDirectory(directory);
            var imagePath = System.IO.Path.Combine(directory, item.ExternalID + ".png");
            bitmap.Save(imagePath);
        }

        private bool ItemExists(int id)
        {
            return repository.GetItem(id) != null;
        }
    }
}

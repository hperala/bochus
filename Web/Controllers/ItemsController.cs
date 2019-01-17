using Core;
using Newtonsoft.Json.Linq;
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

        public ItemsController(IRepository repository)
        {
            this.repository = repository;
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

using Core;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Web.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class RecommendationsController : ApiController
    {
        private IRepository repository;

        public RecommendationsController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/Recommendations
        public IEnumerable<ICategory> GetItems()
        {
            var settings = WebConfigurationManager.AppSettings;
            var numGenres = int.Parse(settings["NumGenres"]);
            var numSubjects = int.Parse(settings["NumSubjects"]);
            var numCategories = int.Parse(settings["NumCategories"]);
            var numItemsInCategory = int.Parse(settings["NumItemsInCategory"]);
            var recommendations = new Recommendations(repository, numGenres, numSubjects);
            return recommendations.GetCategories(numCategories, numItemsInCategory);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Recommendations
    {
        private readonly IRepository repository;

        private readonly int numGenresToUse;

        private readonly int numSubjectsToUse;

        private readonly Random random;

        public Recommendations(IRepository repository, int numGenresToUse, int numSubjectsToUse)
        {
            this.repository = repository;
            this.numGenresToUse = numGenresToUse;
            this.numSubjectsToUse = numSubjectsToUse;
            random = new Random();
        }

        public IEnumerable<ICategory> GetCategories(int numCategories, int numItemsInCategory)
        {
            var allCategories = new List<ICategory>();
            var genres = repository.GetTopGenres(numGenresToUse);
            foreach (var genre in genres)
            {
                allCategories.Add(new GenreBasedCategory(
                    repository,
                    genre,
                    numItemsInCategory));
            }

            var subjects = repository.GetTopSubjects(numSubjectsToUse);
            foreach (var subject in subjects)
            {
                allCategories.Add(new SubjectBasedCategory(
                    repository,
                    subject,
                    numItemsInCategory));
            }

            var chosenCategories = TakeRandom(allCategories, numCategories).ToList();
            foreach (var category in chosenCategories)
            {
                category.LoadItems();
            }
            
            return chosenCategories;
        }

        private IEnumerable<T> TakeRandom<T>(IEnumerable<T> list, int num)
        {
            return list.OrderBy(x => random.Next()).Take(num);
        }
    }
}

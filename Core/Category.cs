using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public interface ICategory
    {
        string Name { get; }
        IEnumerable<Item> Items { get; }

        void LoadItems();
    }

    public class GenreBasedCategory : ICategory
    {
        private readonly IRepository repository;

        private readonly Genre genre;

        private readonly int numItems;

        private List<Item> items = new List<Item>();

        public GenreBasedCategory(IRepository repository, Genre genre, int numItems)
        {
            this.repository = repository;
            this.genre = genre;
            this.numItems = numItems;
            Name = StringUtilities.FirstCharToUpper(genre.Text);
        }

        public string Name { get; }

        public IEnumerable<Item> Items => items;

        public void LoadItems()
        {
            items.AddRange(repository.GetItemsByGenre(genre).Take(numItems));
        }
    }

    public class SubjectBasedCategory : ICategory
    {
        private readonly IRepository repository;

        private readonly Subject subject;

        private readonly int numItems;

        private List<Item> items = new List<Item>();

        public SubjectBasedCategory(IRepository repository, Subject subject, int numItems)
        {
            this.repository = repository;
            this.subject = subject;
            this.numItems = numItems;
            Name = StringUtilities.FirstCharToUpper(subject.Text);
        }

        public string Name { get; }

        public IEnumerable<Item> Items => items;

        public void LoadItems()
        {
            items.AddRange(repository.GetItemsBySubject(subject).Take(numItems));
        }
    }
}

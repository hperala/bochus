using System.Collections.Generic;

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

        private readonly Keyword genre;

        private readonly int numItems;

        private List<Item> items = new List<Item>();

        public GenreBasedCategory(IRepository repository, Keyword genre, int numItems)
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
            items.AddRange(repository.GetRandomItemsByGenre(genre, numItems));
        }
    }

    public class SubjectBasedCategory : ICategory
    {
        private readonly IRepository repository;

        private readonly Keyword subject;

        private readonly int numItems;

        private List<Item> items = new List<Item>();

        public SubjectBasedCategory(IRepository repository, Keyword subject, int numItems)
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
            items.AddRange(repository.GetRandomItemsBySubject(subject, numItems));
        }
    }
}

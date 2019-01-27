using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public interface IRepository
    {
        void AddItem(Item entity);
        Item GetItem(int id);
        IQueryable<Item> GetAllItems();
        IQueryable<Item> GetRandomItemsBySubject(Keyword subject, int limit);
        IQueryable<Item> GetRandomItemsByGenre(Keyword genre, int limit);
        void UpdateItem(Item entity);
        void RemoveItem(Item entity);

        void AddDetailedSubject(DetailedSubject entity);
        DetailedSubject GetDetailedSubject(int id);
        IQueryable<DetailedSubject> GetAllDetailedSubjects();
        void UpdateDetailedSubject(DetailedSubject entity);
        void RemoveDetailedSubject(DetailedSubject entity);

        void AddSubject(Subject entity);
        Subject GetSubject(int id);
        Subject GetSubject(string text);
        IQueryable<Subject> GetAllSubjects();
        IEnumerable<Keyword> GetTopSubjects(int limit);
        void UpdateSubject(Subject entity);
        void RemoveSubject(Subject entity);

        void AddGenre(Genre entity);
        Genre GetGenre(int id);
        Genre GetGenre(string text);
        IQueryable<Genre> GetAllGenres();
        IEnumerable<Keyword> GetTopGenres(int limit);
        void UpdateGenre(Genre entity);
        void RemoveGenre(Genre entity);

        void AddLocation(Location entity);
        Location GetLocation(int id);
        Location GetLocation(string code);
        IQueryable<Location> GetAllLocations();
        void UpdateLocation(Location entity);
        void RemoveLocation(Location entity);

        void SaveChanges();
    }

    public class Keyword
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
    }
}

using Core;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure
{
    public class EFRepository : IRepository
    {
        private readonly BochusDBContext dbContext;
        private readonly EFSubRepository<Item> items;
        private readonly EFSubRepository<DetailedSubject> detailedSubjects;
        private readonly EFSubRepository<Subject> subjects;
        private readonly EFSubRepository<Genre> genres;
        private readonly EFSubRepository<Location> locations;

        public EFRepository()
        {
            dbContext = new BochusDBContext();
            items = new EFSubRepository<Item>(dbContext);
            detailedSubjects = new EFSubRepository<DetailedSubject>(dbContext);
            subjects = new EFSubRepository<Subject>(dbContext);
            genres = new EFSubRepository<Genre>(dbContext);
            locations = new EFSubRepository<Location>(dbContext);
        }

        public void AddDetailedSubject(DetailedSubject entity)
        {
            detailedSubjects.Add(entity);
        }

        public void AddGenre(Genre entity)
        {
            genres.Add(entity);
        }

        public void AddItem(Item entity)
        {
            items.Add(entity);
        }

        public void AddLocation(Location entity)
        {
            locations.Add(entity);
        }

        public void AddSubject(Subject entity)
        {
            subjects.Add(entity);
        }

        public IQueryable<DetailedSubject> GetAllDetailedSubjects()
        {
            return detailedSubjects.GetAll();
        }

        public IQueryable<Genre> GetAllGenres()
        {
            return genres.GetAll();
        }

        public IQueryable<Genre> GetTopGenres(int limit)
        {
            return dbContext.Genres
                .Include(g => g.Items)
                .OrderByDescending(g => g.Items.Count)
                .Take(limit);
        }

        public IQueryable<Item> GetAllItems()
        {
            return items.GetAll();
        }

        public IQueryable<Item> GetItemsBySubject(Subject subject)
        {
            return dbContext.Items
                .Include(i => i.Genres)
                .Include(i => i.Subjects)
                .Include(i => i.Locations)
                .Where(i => i.Subjects.Any(s => s.SubjectID == subject.ID));
        }

        public IQueryable<Item> GetItemsByGenre(Genre genre)
        {
            return dbContext.Items
                .Include(i => i.Genres)
                .Include(i => i.Subjects)
                .Include(i => i.Locations)
                .Where(i => i.Genres.Any(g => g.ID == genre.ID));
        }

        public IQueryable<Location> GetAllLocations()
        {
            return locations.GetAll();
        }

        public IQueryable<Subject> GetAllSubjects()
        {
            return subjects.GetAll();
        }

        public IQueryable<Subject> GetTopSubjects(int limit)
        {
            return dbContext.Subjects
                .Include(s => s.DetailedSubjects)
                .OrderByDescending(s => s.DetailedSubjects.Count)
                .Take(limit);
        }

        public DetailedSubject GetDetailedSubject(int id)
        {
            return detailedSubjects.Get(id);
        }

        public Genre GetGenre(int id)
        {
            return genres.Get(id);
        }

        public Genre GetGenre(string text)
        {
            return dbContext.Genres.Where(g => g.Text == text).FirstOrDefault();
        }

        public Item GetItem(int id)
        {
            return items.Get(id);
        }

        public Location GetLocation(int id)
        {
            return locations.Get(id);
        }

        public Location GetLocation(string code)
        {
            return dbContext.Locations.Where(l => l.Code == code).FirstOrDefault();
        }

        public Subject GetSubject(int id)
        {
            return subjects.Get(id);
        }

        public Subject GetSubject(string text)
        {
            return dbContext.Subjects.Where(s => s.Text == text).FirstOrDefault();
        }

        public void RemoveDetailedSubject(DetailedSubject entity)
        {
            detailedSubjects.Remove(entity);
        }

        public void RemoveGenre(Genre entity)
        {
            genres.Remove(entity);
        }

        public void RemoveItem(Item entity)
        {
            items.Remove(entity);
        }

        public void RemoveLocation(Location entity)
        {
            locations.Remove(entity);
        }

        public void RemoveSubject(Subject entity)
        {
            subjects.Remove(entity);
        }

        public void UpdateDetailedSubject(DetailedSubject entity)
        {
            detailedSubjects.Update(entity);
        }

        public void UpdateGenre(Genre entity)
        {
            genres.Update(entity);
        }

        public void UpdateItem(Item entity)
        {
            items.Update(entity);
        }

        public void UpdateLocation(Location entity)
        {
            locations.Update(entity);
        }

        public void UpdateSubject(Subject entity)
        {
            subjects.Update(entity);
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }
    }
}

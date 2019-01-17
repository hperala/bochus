using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class Item
    {
        public int ID { get; set; }
        public string AuthorFirstNames { get; set; }
        public string AuthorLastName { get; set; }

        [NotMapped]
        public string AuthorName
        {
            get
            {
                return AuthorFirstNames + " " + AuthorLastName;
            }
        }

        public string OtherAuthors { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Publisher { get; set; }
        public string PublicationYear { get; set; }
        public string FormatCode { get; set; }
        public string Isbn { get; set; }
        public List<string> Series { get; set; }
        public string ExternalID { get; set; }
        public string ExternalRelativeUrl { get; set; }
        public List<DetailedSubject> Subjects { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Location> Locations { get; set; }
    }

    public class DetailedSubject
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int SubjectID { get; set; }
        public string FullText { get; set; }

        [JsonIgnore]
        public Item Item { get; set; }

        [JsonIgnore]
        public Subject Subject { get; set; }
    }

    public class Subject
    {
        public int ID { get; set; }
        public string Text { get; set; }

        [JsonIgnore]
        public List<DetailedSubject> DetailedSubjects { get; set; }
    }

    public class Genre
    {
        public int ID { get; set; }
        public string Text { get; set; }

        [JsonIgnore]
        public List<Item> Items { get; set; }
    }

    public class Location
    {
        public int ID { get; set; }
        public string ParentLocationCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<Item> Items { get; set; }
    }
}

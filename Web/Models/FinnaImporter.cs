using Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Web
{
    public class FinnaImporter
    {
        private const string PropNotFound = "Required property not found: \"{0}\"";
        private const string ArrayEmpty = "Unexpected empty array: \"{0}\"";
        private const string InvalidFormat = "Unable to parse value: \"{0}\"";
        private const int LibraryLevel = 2;

        private readonly IRepository repository;

        private readonly string subjectPartSeparator;

        public FinnaImporter(IRepository repository, string subjectPartSeparator)
        {
            this.repository = repository;
            this.subjectPartSeparator = subjectPartSeparator;
        }

        public Item CreateItem(JObject finnaItem)
        {
            var item = new Item();
            ExtractAuthors(finnaItem, item);
            ExtractTitle(finnaItem, item);
            ExtractSeries(finnaItem, item);
            ExtractFormat(finnaItem, item);
            ExtractMiscFields(finnaItem, item);
            ExtractSubjects(finnaItem, item);
            ExtractGenres(finnaItem, item);
            ExtractLocations(finnaItem, item);
            return item;
        }

        private void ExtractAuthors(JObject finnaItem, Item item)
        {
            var authors = ArrayOrThrow(finnaItem, "primaryAuthors");
            if (authors.Count == 0)
            {
                authors = NonEmptyArrayOrThrow(finnaItem, "secondaryAuthors");
            }

            dynamic firstAuthor = ParseName(authors[0]);
            item.AuthorFirstNames = firstAuthor.FirstNames;
            item.AuthorLastName = firstAuthor.LastName;

            var otherAuthors = new List<string>();
            for (var i = 1; i < authors.Count; i++)
            {
                dynamic otherAuthor = ParseName(authors[i]);
                otherAuthors.Add(otherAuthor.FirstNames + " " + otherAuthor.LastName);
            }
            if (otherAuthors.Count > 0)
            {
                item.OtherAuthors = string.Join(", ", otherAuthors);
            }
        }

        private void ExtractTitle(JObject finnaItem, Item item)
        {
            var title = StringOrThrow(finnaItem, "title");
            item.Title = title;

            var originalTitle = FirstStringOrNull(finnaItem, "uniformTitles");
            if (originalTitle == null)
            {
                return;
            }
            
            var commonLangs = new string[] { ", suomi" };
            foreach (var lang in commonLangs)
            {
                var pos = originalTitle.LastIndexOf(lang);
                if (pos != -1)
                {
                    originalTitle = originalTitle.Substring(0, pos);
                    break;
                }
            }
            item.OriginalTitle = originalTitle;
        }

        private void ExtractSeries(JObject finnaItem, Item item)
        {
            var listToken = finnaItem["series"];
            if (listToken == null)
            {
                return;
            }
            var list = (JArray)listToken;
            if (list.Count == 0)
            {
                return;
            }

            var result = new List<string>();
            foreach (var seriesToken in list)
            {
                var name = StringOrThrow(seriesToken, "name");
                if (seriesToken["additional"] != null)
                {
                    result.Add((string)seriesToken["additional"]);
                }
                else
                {
                    result.Add(name);
                }
            }

            if (result.Count > 0)
            {
                item.Series = result;
            }
        }

        private void ExtractFormat(JObject finnaItem, Item item)
        {
            var formats = (JArray)finnaItem["formats"];
            if (formats == null || formats.Count == 0)
            {
                return;
            }
            var firstFormat = formats[0];
            var value = StringOrThrow(firstFormat, "value");
            item.FormatCode = (string)value;
        }

        private void ExtractMiscFields(JObject finnaItem, Item item)
        {
            item.Publisher = FirstStringOrNull(finnaItem, "publishers");
            item.PublicationYear = StringOrNull(finnaItem, "year");
            item.Isbn = StringOrNull(finnaItem, "cleanIsbn");
            item.ExternalID = StringOrNull(finnaItem, "id");
            item.ExternalRelativeUrl = StringOrNull(finnaItem, "recordPage");
        }

        private void ExtractSubjects(JObject finnaItem, Item item)
        {
            var subjectTokens = ArrayOrThrow(finnaItem, "subjects");

            var subjects = new List<DetailedSubject>();
            foreach (var subjectToken in subjectTokens)
            {
                var subjectPartTokens = (JArray)subjectToken;
                if (subjectPartTokens.Count == 0)
                {
                    continue;
                }
                var firstPart = (string)subjectPartTokens[0];
                var joinedParts = string.Join(subjectPartSeparator, subjectPartTokens);

                var genericSubject = LoadOrCreateSubject(firstPart);
                var subject = new DetailedSubject
                {
                    FullText = joinedParts,
                    Subject = genericSubject
                };
                subjects.Add(subject);
            }

            item.Subjects = subjects;
        }

        private void ExtractGenres(JObject finnaItem, Item item)
        {
            var genreTokens = (JArray)finnaItem["genres"];
            if (genreTokens == null || genreTokens.Count == 0)
            {
                return;
            }

            var genreNames = genreTokens.ToObject<IList<string>>();
            var genres = from name in genreNames
                         select LoadOrCreateGenre(name);

            item.Genres = genres.ToList();
        }

        private void ExtractLocations(JObject finnaItem, Item item)
        {
            var buildings = (JArray)finnaItem["buildings"];
            if (buildings == null || buildings.Count == 0)
            {
                return;
            }

            var locations = new List<Location>();
            foreach (var buildingToken in buildings)
            {
                var value = StringOrThrow(buildingToken, "value");
                var translated = StringOrThrow(buildingToken, "translated");

                dynamic buildingCode = ParseBuilding(value);
                if (buildingCode.Level == LibraryLevel)
                {
                    var location = LoadOrCreateLocation(buildingCode.ParentBuilding, translated, value);
                    locations.Add(location);
                }
            }

            item.Locations = locations;
        }

        private Subject LoadOrCreateSubject(string text)
        {
            var subject = repository.GetSubject(text);
            if (subject == null)
            {
                subject = new Subject
                {
                    Text = text
                };
            }
            return subject;
        }

        private Genre LoadOrCreateGenre(string name)
        {
            var genre = repository.GetGenre(name);
            if (genre == null)
            {
                genre = new Genre
                {
                    Text = name
                };
            }
            return genre;
        }

        private Location LoadOrCreateLocation(string parentBuilding, string translated, string value)
        {
            var location = repository.GetLocation(value);
            if (location == null)
            {
                location = new Location
                {
                    ParentLocationCode = parentBuilding,
                    Name = translated,
                    Code = value
                };
            }
            return location;
        }

        private JArray NonEmptyArrayOrThrow(JToken token, string propertyName)
        {
            var array = ArrayOrThrow(token, propertyName);
            if (array.Count == 0)
            {
                throw new ArgumentException(string.Format(ArrayEmpty, propertyName));
            }
            return array;
        }

        private JArray ArrayOrThrow(JToken token, string propertyName)
        {
            var array = (JArray)token[propertyName];
            if (array == null)
            {
                throw new ArgumentException(string.Format(PropNotFound, propertyName));
            }
            return array;
        }

        private string StringOrThrow(JToken token, string propertyName)
        {
            var prop = token[propertyName];
            if (prop == null)
            {
                throw new ArgumentException(string.Format(PropNotFound, propertyName));
            }
            return (string)prop;
        }

        private string FirstStringOrNull(JToken token, string propertyName)
        {
            var array = (JArray)token[propertyName];
            if (array != null)
            {
                if (array.Count > 0)
                {
                    return (string)array[0];
                }
            }
            return null;
        }

        private string StringOrNull(JToken token, string propertyName)
        {
            return (string)token[propertyName];
        }

        private object ParseName(JToken nameToken)
        {
            string lastName;
            string firstNames;
            
            var name = (string)nameToken;
            name = name.Replace(", kirjoittaja", "");
            name = name.Replace(", toimittaja", "");
            name = name.Replace(", kääntäjä", "");

            int endOfLastNamePos = name.IndexOf(",");
            if (endOfLastNamePos != -1)
            {
                lastName = name.Substring(0, endOfLastNamePos);
                firstNames = name.Substring(endOfLastNamePos + 1).TrimStart();
            }
            else
            {
                lastName = name;
                firstNames = "";
            }

            return new { FirstNames = firstNames, LastName = lastName };
        }

        private object ParseBuilding(string building)
        {
            var parts = building.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                throw new ArgumentException(string.Format(InvalidFormat, building));
            }

            var level = int.Parse(parts[0]);
            var parentPath = parts.Take(parts.Length - 1).Skip(1);
            var parent = string.Format("{0}/{1}/", level - 1, string.Join("/", parentPath));

            return new { Level = level, ParentBuilding = parent };
        }
    }
}
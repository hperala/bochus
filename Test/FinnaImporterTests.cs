using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Moq;
using Core;
using Web;

namespace WebApp.Models.Tests
{
    [TestClass()]
    public class FinnaImporterTests
    {
        string data = @"
        {
            ""authors"": {
                ""primary"": {
                    ""Harris, Neil Patrick, kirjoittaja"": {
                        ""role"": [
                            ""kirjoittaja""
                        ]
                    }
                },
                ""secondary"": {
                    ""Marlin, Lissy, kuvittaja"": {
                        ""role"": [
                            ""kuvittaja""
                        ]
                    },
                    ""Hilton, Kyle, kuvittaja"": {
                        ""role"": [
                            ""k\u00e4\u00e4nt\u00e4j\u00e4""
                        ]
                    },
                    ""Parviainen, Sirpa, k\u00e4\u00e4nt\u00e4j\u00e4"": {
                        ""role"": [
                            ""-""
                        ]
                    }
                },
                ""corporate"": []
            },
            ""awards"": [],
            ""buildings"": [
                {
                    ""value"": ""0\/Heili\/"",
                    ""translated"": ""Heili-kirjastot""
                },
                {
                    ""value"": ""1\/Heili\/1\/"",
                    ""translated"": ""Lappeenranta""
                },
                {
                    ""value"": ""1\/Heili\/2\/"",
                    ""translated"": ""Savitaipale""
                },
                {
                    ""value"": ""1\/Heili\/5\/"",
                    ""translated"": ""Imatra""
                },
                {
                    ""value"": ""2\/Heili\/1\/001\/"",
                    ""translated"": ""Lappeenrannan p\u00e4\u00e4kirjasto""
                },
                {
                    ""value"": ""2\/Heili\/2\/101\/"",
                    ""translated"": ""Savitaipaleen kirjasto""
                },
                {
                    ""value"": ""2\/Heili\/5\/201\/"",
                    ""translated"": ""Imatran p\u00e4\u00e4kirjasto""
                }
            ],
            ""cleanIsbn"": ""9527269350"",
            ""formats"": [
                {
                    ""value"": ""0\/Book\/"",
                    ""translated"": ""Kirja""
                },
                {
                    ""value"": ""1\/Book\/Book\/"",
                    ""translated"": ""Kirja""
                }
            ],
            ""genres"": [
                ""lastenkirjallisuus"",
                ""romaanit"",
                ""kaunokirjallisuus englanninkielinen kirjallisuus k\u00e4\u00e4nn\u00f6kset""
            ],
            ""humanReadablePublicationDates"": [
                ""[2018]""
            ],
            ""id"": ""heili.1213456"",
            ""institutions"": [
                {
                    ""value"": ""Heili"",
                    ""translated"": ""Heili-kirjastot""
                }
            ],
            ""isbns"": [
                ""978-952-7269-35-0 sidottu""
            ],
            ""languages"": [
                ""fin""
            ],
            ""nonPresenterAuthors"": [
                {
                    ""name"": ""Harris, Neil Patrick"",
                    ""role"": ""kirjoittaja.""
                },
                {
                    ""name"": ""Marlin, Lissy"",
                    ""role"": ""kuvittaja.""
                },
                {
                    ""name"": ""Hilton, Kyle"",
                    ""role"": ""kuvittaja.""
                },
                {
                    ""name"": ""Parviainen, Sirpa"",
                    ""role"": ""k\u00e4\u00e4nt\u00e4j\u00e4.""
                }
            ],
            ""onlineUrls"": [],
            ""originalLanguages"": [
                ""eng""
            ],
            ""placesOfPublication"": [
                ""Helsinki :""
            ],
            ""previousTitles"": [],
            ""primaryAuthors"": [
                ""Harris, Neil Patrick, kirjoittaja""
            ],
            ""publicationDates"": [
                ""2018""
            ],
            ""publicationEndDate"": ""2018"",
            ""publicationFrequency"": [],
            ""publicationInfo"": [
                ""Helsinki :""
            ],
            ""publishers"": [
                ""Aula & Co""
            ],
            ""recordPage"": ""\/Record\/heili.1213456"",
            ""secondaryAuthors"": [
                ""Marlin, Lissy, kuvittaja"",
                ""Hilton, Kyle, kuvittaja"",
                ""Parviainen, Sirpa, k\u00e4\u00e4nt\u00e4j\u00e4""
            ],
            ""series"": [],
            ""shortTitle"": ""Taikajengi"",
            ""subjects"": [
                [
                    ""Carter"",
                    ""(fiktiivinen hahmo)""
                ],
                [
                    ""seikkailu""
                ],
                [
                    ""magia""
                ],
                [
                    ""taikatemput""
                ],
                [
                    ""taikurit""
                ],
                [
                    ""varkaat""
                ],
                [
                    ""tivolit""
                ],
                [
                    ""yst\u00e4v\u00e4t"",
                    ""jengit"",
                    ""Taikajengi""
                ],
                [
                    ""pikkukaupungit"",
                    ""Ihmel\u00e4hteiden kaupunki""
                ]
            ],
            ""title"": ""Taikajengi"",
            ""titleStatement"": ""Neil Patrick Harris ; suomentanut Sirpa Parviainen ; story illustrations by Lissy Marlin ; how-to illustrations by Kyle Hilton"",
            ""uniformTitles"": [
                ""The magic misfits, suomi""
            ],
            ""year"": ""2018""
        }
";
        JObject dataJson;
        Item item;
        FinnaImporter importer;

        [TestInitialize()]
        public void SetUp()
        {
            dataJson = JObject.Parse(data);
            var repo = new Mock<IRepository>();
            repo.Setup(r => r.GetSubject(It.IsAny<string>())).Returns((Subject)null);
            repo.Setup(r => r.GetLocation(It.IsAny<string>())).Returns((Location)null);
            repo.Setup(r => r.GetGenre(It.IsAny<string>())).Returns((Genre)null);
            importer = new FinnaImporter(repo.Object, " : ");
            item = importer.CreateItem(dataJson);
        }

        [TestMethod()]
        public void NameTest()
        {
            Assert.AreEqual("Harris", item.AuthorLastName, "Last name should be extracted and role removed");
            Assert.AreEqual("Neil Patrick", item.AuthorFirstNames, "First names should be extracted");
        }

        [TestMethod()]
        public void TitleTest()
        {
            Assert.AreEqual("Taikajengi", item.Title, "Title field should contain full title");
            Assert.AreEqual("The magic misfits", item.OriginalTitle, "Original title should be extracted and language of translation removed");
        }

        [TestMethod()]
        public void OtherFieldTest()
        {
            Assert.AreEqual("Aula & Co", item.Publisher);
            Assert.AreEqual("2018", item.PublicationYear);
            Assert.AreEqual("0/Book/", item.FormatCode);
            Assert.AreEqual("9527269350", item.Isbn);
            Assert.IsNull(item.Series);
            Assert.AreEqual("heili.1213456", item.ExternalID);
            Assert.AreEqual("/Record/heili.1213456", item.ExternalRelativeUrl);
        }

        [TestMethod()]
        public void SubjectTest()
        {
            var detSubjects = item.Subjects;
            detSubjects.Sort((x, y) => x.FullText.CompareTo(y.FullText));

            Assert.AreEqual(9, detSubjects.Count);
            Assert.AreEqual("Carter : (fiktiivinen hahmo)", detSubjects[0].FullText);
            Assert.AreEqual("Carter", detSubjects[0].Subject.Text);
            Assert.AreEqual("ystävät : jengit : Taikajengi", detSubjects[8].FullText);
            Assert.AreEqual("ystävät", detSubjects[8].Subject.Text);
        }

        [TestMethod()]
        public void GenreTest()
        {
            var genres = item.Genres;
            genres.Sort((x, y) => x.Text.CompareTo(y.Text));

            Assert.AreEqual(3, genres.Count);
            Assert.AreEqual("kaunokirjallisuus englanninkielinen kirjallisuus käännökset", genres[0].Text);
            Assert.AreEqual("lastenkirjallisuus", genres[1].Text);
            Assert.AreEqual("romaanit", genres[2].Text);
        }

        [TestMethod()]
        public void LocationTest()
        {
            var locs = item.Locations;
            locs.Sort((x, y) => x.Code.CompareTo(y.Code));

            Assert.AreEqual(3, locs.Count);
            Assert.AreEqual("2/Heili/1/001/", locs[0].Code);
            Assert.AreEqual("Lappeenrannan pääkirjasto", locs[0].Name);
            Assert.AreEqual("1/Heili/1/", locs[0].ParentLocationCode);
            Assert.AreEqual("2/Heili/2/101/", locs[1].Code);
            Assert.AreEqual("Savitaipaleen kirjasto", locs[1].Name);
            Assert.AreEqual("1/Heili/2/", locs[1].ParentLocationCode);
            Assert.AreEqual("2/Heili/5/201/", locs[2].Code);
            Assert.AreEqual("Imatran pääkirjasto", locs[2].Name);
            Assert.AreEqual("1/Heili/5/", locs[2].ParentLocationCode);
        }

        [TestMethod()]
        public void NoPrimaryAuthorTest()
        {
            var data = @"{ 
                ""primaryAuthors"": [], 
                ""title"": ""Naamiot"",
                ""secondaryAuthors"": [
                    ""Kumara-Moisio, Taru, toimittaja"",
                    ""Leinonen, Anne, toimittaja"",
                    ""Roininen, Minna, toimittaja""
                ],
                ""subjects"": [],
            }";
            dataJson = JObject.Parse(data);
            item = importer.CreateItem(dataJson);

            Assert.AreEqual("Taru Kumara-Moisio", item.AuthorName, "The secondaryAuthors field should be used if there is no primary author");
        }
    }
}
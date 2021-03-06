﻿using Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    class Program
    {
        static string[] Commands = new string[] { "get-finna", "import-finna" };

        enum CommandIndex { GetFinna, ImportFinna }

        static void Main(string[] args)
        {
            try
            {
                if (!ValidArguments(args))
                {
                    Usage();
                    return;
                }

                var command = args[0];
                if (command == Commands[(int)CommandIndex.GetFinna])
                {
                    GetFinnaDataAsync().Wait();
                }
                else if (command == Commands[(int)CommandIndex.ImportFinna])
                {
                    ImportFinnaDataAsync(args).Wait();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine(e.InnerException.Message);
                }
            }
        }

        static void Usage()
        {
            var syntaxFormat = "Usage: mgttool ({0}) <arguments>";
            var syntax = string.Format(syntaxFormat, string.Join("|", Commands));
            Console.WriteLine(syntax);
            Console.WriteLine("Commands:");
            Console.WriteLine($"\t {Commands[(int)CommandIndex.GetFinna]}");
            Console.WriteLine("\t\tDownload predefined data from the Finna API and save it locally.");
            Console.WriteLine($"\t {Commands[(int)CommandIndex.ImportFinna]} <url> <file to import>");
            Console.WriteLine("\t\tSend locally saved Finna data to the application's Import endpoint.");
            Console.WriteLine("\t\tA mandatory password parameter will be added from configuration.");
            Console.WriteLine("\t\tE.g. mgttool import-finna http://localhost:53133/api/Items/Import response-fantasia-2018.json");
        }

        static bool ValidArguments(string[] args)
        {
            return args.Length > 0
                && Commands.Contains(args[0]);
        }

        static async Task GetFinnaDataAsync()
        {
            var settings = ConfigurationManager.AppSettings;
            var cooldown = settings["FinnaCooldownMillis"];
            var finnaApiAccess = new Throttler()
            {
                MinIntervalMillis = int.Parse(cooldown)
            };

            using (var client = new HttpClient())
            {
                var search = CreateSearch(settings);
                search.Genre = "tieteiskirjallisuus";
                search.PublishDate = "2018";
                await SaveFirstPageAsync(search, client, finnaApiAccess, "-scifi-2018");

                search = CreateSearch(settings);
                search.Genre = "fantasiakirjallisuus";
                search.PublishDate = "2018";
                await SaveFirstPageAsync(search, client, finnaApiAccess, "-fantasia-2018");

                search = CreateSearch(settings);
                search.Limit = "50";
                search.LookFor = "ohjelmointi";
                search.Type = "Subject";
                await SaveFirstPageAsync(search, client, finnaApiAccess, "-ohjelmointi");
            }
        }

        static async Task ImportFinnaDataAsync(string[] args)
        {
            var settings = ConfigurationManager.AppSettings;
            var password = settings["AdminPassword"];
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Setting \"AdminPassword\" not found.");
            }
            var url = BuildImportUrl(args[1], password);
            var fileName = args[2];

            var root = JObject.Parse(File.ReadAllText(fileName));
            var records = (JArray)root["records"];
            Console.WriteLine($"Reading {records.Count} records from file.");

            using (var client = new HttpClient())
            {
                foreach (var recordToken in records)
                {
                    try
                    {
                        var serialized = recordToken.ToString();
                        var request = new HttpRequestMessage(HttpMethod.Post, url);
                        request.Content = new StringContent(serialized, Encoding.UTF8, "application/json");
                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        Console.Write(".");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\r\n" + e.Message);
                    }
                }
            }
        }

        static FinnaApiSearch CreateSearch(NameValueCollection settings)
        {
            var baseUrl = settings["FinnaBaseUrl"];
            var building = settings["FinnaBuildingParam"];
            var resultsPerPage = settings["FinnaResultsPerPage"];
            var fields = settings["FinnaFields"].Split(',').ToList();
            var format = settings["FinnaFormat"];
            var search = new FinnaApiSearch()
            {
                BaseUrl = baseUrl,
                Building = building,
                Limit = resultsPerPage,
                Fields = fields,
            };
            if (format != null)
            {
                search.SetFilter("format", format);
            }
            return search;
        }

        static async Task SaveFirstPageAsync(
            FinnaApiSearch search,
            HttpClient client,
            Throttler api,
            string id)
        {
            while (!api.IsAvailable(DateTime.Now))
            {
                Thread.Sleep(api.MinIntervalMillis);
            }

            var request = search.CreateRequest();
            Console.WriteLine("Request: " + request.RequestUri.ToString());

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode.ToString() + " " + response.ReasonPhrase);
            }
            if (response.Content != null)
            {
                var content = await response.Content.ReadAsStringAsync();
                File.WriteAllText($"response{id}.json", content);
            }
            api.RequestCompleted(DateTime.Now);
        }

        static string BuildImportUrl(string urlWithoutQuery, string password)
        {
            var query = UrlUtilities.ToQuery(
                new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("password", password)
                },
                false
            );
            return urlWithoutQuery + "?" + query;
        }
    }
}

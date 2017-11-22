using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace AquaBot
{
    public class WikipediaClient
    {
        private static string endpoint = @"https://en.wikipedia.org/w/api.php";
        private static string wikipediaBaseUrl = @"http://en.wikipedia.org/wiki/";

        public async Task<string> SearchForPage(string searchTerm)
        {
            try
            {
                var requestTerms = $"?action=query&list=search&srsearch={HttpUtility.UrlEncode(searchTerm)}&format=json";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Api-User-Agent", "AquaBot/1.0");
                var result = await client.GetAsync(endpoint + requestTerms);
                var resultContent = await result.Content.ReadAsStringAsync();
                var resultObject = JsonConvert.DeserializeObject<RootObject>(resultContent);
                if (resultObject.query.search.Count > 0)
                {
                    return MakeWikipediaURLFromTitle(resultObject.query.search.FirstOrDefault().title);
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                // Pro error handling
                return "BROKE";
            }
        }

        // Because fuck making a url avaliable in our api?? GG Wikipedia
        private string MakeWikipediaURLFromTitle(string title) => wikipediaBaseUrl + HttpUtility.UrlEncode(title.Replace(" ", "_"));
    }

    internal class Query
    {
        public List<pageval> search { get; set; }
    }

    internal class RootObject
    {
        public string batchcomplete { get; set; }
        public Query query { get; set; }
    }

    internal class pageval
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public int size { get; set; }
        public string snippet { get; set; }
        public DateTime timestamp { get; set; }
    }
}
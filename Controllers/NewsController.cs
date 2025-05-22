using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using NewsReaderAPI.Models;

namespace NewsReaderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public NewsController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required!");

            string apiKey = _configuration["NewsApi:ApiKey"];
            string url = $"https://newsapi.org/v2/everything?q={keyword}&apiKey={apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Error getting NewsApi");

            var jsonString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonString);

            var articles = json["articles"];

            var newList = new List<NewsArticle>();

            foreach (var article in articles)
            {
                newList.Add(new NewsArticle
                {
                    Title = (string)article["title"],
                    Description = (string)article["description"],
                    Url = (string)article["url"],
                    UrlToImage = (string)article["urlToImage"],
                    PublishedAt = (string)article["publishedAt"],
                });
            }
            return Ok(newList);

        }
    }
}

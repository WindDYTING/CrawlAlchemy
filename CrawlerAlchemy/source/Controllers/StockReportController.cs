using System.Text;
using CrawlerAlchemy.Options;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CrawlerAlchemy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockReportController : ControllerBase
    {
        private readonly IHttpClientFactory _factory;
        private readonly UserOption _userOption;
        private const string WebUrl = "https://investanchors.com/user/vip_contents/investanchors_index?lv=super_vip";
        private HtmlWeb _web = new();

        public StockReportController(IHttpClientFactory factory, IOptions<UserOption> userOption)
        {
            _factory = factory;
            _userOption = userOption.Value;
        }

        [HttpGet]
        public async Task<ActionResult> GetReportsAsync(CancellationToken token)
        {
            using var client = _factory.CreateClient();
            var respOfLogin = await client.GetAsync("https://investanchors.com/user/register/new", token);
            respOfLogin.EnsureSuccessStatusCode();
            var htmlOfLogin = await respOfLogin.Content.ReadAsStringAsync(token);
            var htmlLoader = new HtmlDocument();
            htmlLoader.LoadHtml(htmlOfLogin);

            var authenticityTokenNode = htmlLoader.DocumentNode.SelectSingleNode(
                "//*[@id=\"Paris\"]/div/div/div/div/div/form/input");
            var authenticityToken = authenticityTokenNode.Attributes["value"].Value;
            var content = new MultipartFormDataContent
            {
                {new StringContent(_userOption.Email), "user[email]"},
                {new StringContent(_userOption.Password), "user[password]"},
                {new StringContent(authenticityToken), "authenticity_token"}
            };
            var req = new HttpRequestMessage(HttpMethod.Post, "https://investanchors.com/user/register")    
            {
                Content = content
            };
            var resp = await client.SendAsync(req, token);  
            resp.EnsureSuccessStatusCode();

            var doc = await client.GetStringAsync(WebUrl, token);

            htmlLoader.LoadHtml(doc);
            var nodes = htmlLoader.DocumentNode.SelectNodes("//*[@id=\"content\"]/section/div/div/div[2]/div[1]/div[2]/table/tbody");

            if (nodes is null || !nodes.Any())
            {
                return NotFound();
            }

            return Ok();
        }
    }
}

using CrawlerAlchemy.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PuppeteerSharp;

namespace CrawlerAlchemy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockReportController : ControllerBase
    {
        private readonly UserOption _userOption;
        private const string WebUrl = "https://investanchors.com/user/vip_contents/investanchors_index?lv=super_vip";
        private const string WeekNewsUrl = "https://investanchors.com/user/vip_contents/investanchors_index?lv=weekly";
        private const string LoginUrl = "https://investanchors.com/user/register/new";
        private readonly IBrowser _browser;

        public StockReportController(IOptions<UserOption> userOption, IBrowser browser)
        {
            _browser = browser;
            _userOption = userOption.Value;
        }

        private async Task<IPage> LoginAsync()
        {
            var page = await _browser.NewPageAsync();
            await page.GoToAsync(LoginUrl);
            await page.TypeAsync("#London > div > div > div > div > div > form > div:nth-child(3) > input", _userOption.Email);
            await page.TypeAsync("#London > div > div > div > div > div > form > div:nth-child(5) > input", _userOption.Password);
            await page.ClickAsync("#London > div > div > div > div > div > form > button");
            await page.WaitForNavigationAsync(new NavigationOptions());
            return page;
        }

        [HttpGet]
        public async Task<ActionResult> GetReportsAsync(CancellationToken token)
        {
            var resp = await LoginAsync();
            Checks.EnsureNotNull(resp);
            await resp.GoToAsync(WeekNewsUrl);
            await resp.ClickAsync("#content > section > div > div > div.col-md-offset-1.col-lg-offset-1.col-md-10.col-lg-10 > div.subs-free.user-rig.border-top-none.subs-vip > div.vip-top-box.row > div:nth-child(2) > div > div:nth-child(2) > p");
            
            var table = await resp.QuerySelectorAllAsync("#content > section > div > div > div.col-md-offset-1.col-lg-offset-1.col-md-10.col-lg-10 > div.subs-free.user-rig.border-top-none.subs-vip > table > tbody");

            //foreach (var elementHandle in table)
            //{
            //    var selectAsync = await elementHandle.JsonValueAsync();
            //    Console.WriteLine(selectAsync);
            //}


            return Ok();
        }
    }
}

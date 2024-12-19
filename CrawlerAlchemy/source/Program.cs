using CrawlerAlchemy.Options;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<UserOption>()
    .Configure(option =>
    {
        builder.Configuration.GetSection("user").Bind(option);
    });
builder.Services.AddSingleton<BrowserFetcher>();
builder.Services.AddSingleton(sp =>
{
    var fetcher = sp.GetRequiredService<BrowserFetcher>();
    fetcher.DownloadAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    return Puppeteer.LaunchAsync(
        new LaunchOptions { Headless = true }).ConfigureAwait(false).GetAwaiter().GetResult();
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using CrawlerAlchemy.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<UserOption>()
    .Configure(option =>
    {
        builder.Configuration.GetSection("user").Bind(option);
    });

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("CrawlerClient")
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

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

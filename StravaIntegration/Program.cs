using StravaIntegration.Configuration;
using StravaIntegration.Services;
using StravaIntegration.Endpoints;
using StravaIntegration.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.Configure<StravaOptions>(
    builder.Configuration.GetSection(StravaOptions.SectionName));

builder.Services.AddHttpClient<IStravaClient, StravaClient>(client =>
{
    client.BaseAddress = new Uri("https://www.strava.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "StravaIntegration/1.0");
});

builder.Services.AddScoped<IStravaAuthService, StravaAuthService>();
builder.Services.AddScoped<IStravaActivityService, StravaActivityService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapStravaEndpoints();

app.Run();
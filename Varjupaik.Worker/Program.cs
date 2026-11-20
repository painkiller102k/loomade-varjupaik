using Varjupaik.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Seaded failist appsettings.json
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection("Worker"));

// HttpClient API jaoks
builder.Services.AddHttpClient("api", (services, client) =>
{
    var api = builder.Configuration.GetSection("Api").Get<ApiOptions>() ?? new ApiOptions();
    client.BaseAddress = new Uri(api.BaseUrl);
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

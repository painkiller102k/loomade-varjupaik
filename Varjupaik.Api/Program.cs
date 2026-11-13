using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Varjupaik.Api.Auth;
using Varjupaik.Api.ErrorHandling;
using Varjupaik.Api.WebSockets;
using Varjupaik.Services;
using Varjupaik.Services.Data;
using Varjupaik.Services.Notifications;
using Varjupaik.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Andmebaas (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Repositooriumid
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVetVisitRepository, VetVisitRepository>();

// Teenused
// Reaalajas teavitused (WebSocket)
builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddSingleton<IApplicationNotifier, WebSocketApplicationNotifier>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AnimalService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<VetVisitService>();
builder.Services.AddScoped<TokenService>();

// JWT autentimine
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Brauseri WebSocket ei saa Authorization päist saata, seega loeme tokeni query-parameetrist
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) && context.HttpContext.Request.Path.StartsWithSegments("/ws"))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

// CORS: lubame päringud Reacti kliendilt (Vite dev server)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Ühtne veakäsitlus: kõik vead tagastatakse ProblemDetails kujul
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .ConfigureApiBehaviorOptions(options =>
    {
        // Vigased andmed (valideerimine) tagastatakse samuti application/problem+json kujul
        options.InvalidModelStateResponseFactory = context =>
        {
            var factory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problem = factory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);
            return new ContentResult
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ContentType = "application/problem+json",
                Content = System.Text.Json.JsonSerializer.Serialize(problem, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web))
            };
        };
    });

// Swagger koos XML kommentaaride ja JWT Authorize nupuga
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Loomade varjupaik API",
        Version = "v1",
        Description = "Varjupaiga loomade ja adopteerimise taotluste haldamine"
    });

    // XML kommentaarid mõlemast projektist
    foreach (var xml in Directory.GetFiles(AppContext.BaseDirectory, "Varjupaik.*.xml"))
    {
        options.IncludeXmlComments(xml);
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Kleebi siia token, mille said /api/auth/login päringust"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

var app = builder.Build();

// Rakendame migratsioonid käivitamisel
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var seedUsers = builder.Configuration.GetSection("SeedUsers").Get<List<SeedUser>>() ?? new();
    DbSeeder.SeedUsers(db, seedUsers);
    DbSeeder.SeedAnimals(db);
    DbSeeder.SeedVetData(db);
}

// Erandid -> ProblemDetails; tühjad 401/403/404 vastused saavad samuti ProblemDetails sisu
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Üles laaditud pildid wwwroot/uploads kaustast
app.UseStaticFiles();

app.UseCors("Frontend");
app.UseWebSockets();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapApplicationsSocket();

app.Run();

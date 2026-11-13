using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Varjupaik.Api.Auth;
using Varjupaik.Services;
using Varjupaik.Services.Data;
using Varjupaik.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Andmebaas (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Repositooriumid
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Teenused
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AnimalService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<TokenService>();

// JWT autentimine
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Üles laaditud pildid wwwroot/uploads kaustast
app.UseStaticFiles();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

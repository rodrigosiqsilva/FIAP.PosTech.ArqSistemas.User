using Microsoft.OpenApi.Models;
using FIAP.PosTech.ArqSistemas.UserAPI.Middlewares;
using FIAP.PosTech.ArqSistemas.UserAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Configurar logging estruturado
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers();

// Register Usuario Service
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();

// Configure OpenAPI/Swagger using Swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FIAP Cloud Games (FCG)",
        Version = "v1",
        Description = "A FIAP Cloud Games (FCG) é uma plataforma de venda de jogos digitais e gestão de servidores para partidas on-line",
        Contact = new OpenApiContact
        {
            Name = "Rodrigo Siqueira Silva",
            Email = "rodrigosiqueirasilva@hotmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization - Por favor, insira 'Bearer' e o token JWT. Exemplo: 'Bearer 12345abcdef'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              }
            },
            new List<string>()
          }
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"));
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("User", policy => policy.RequireRole("User"));


var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP Cloud Games (FCG) v1");
    c.RoutePrefix = "swagger";
});

// Usar middleware de correlationId e tratamento de erros global
app.UseCorrelationId();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Log ao iniciar a aplicação
logger.LogInformation("Iniciando aplicação FIAP Cloud Games (FCG)");

try
{
    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Aplicação encerrada com erro");
}


using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Configure OpenAPI/Swagger using Swashbuckle
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Register Swagger generator and explorer so the Swagger UI is available at /swagger
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
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

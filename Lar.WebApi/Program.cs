using FluentValidation.AspNetCore;
using Lar.Infra.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Text;
using Lar.WebApi.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Cadastro de pessoas",
        Description = "Uma API contendo funcionalidades para o cadastro de pessoas no sistema.",
        TermsOfService = new Uri("https://www.example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Nícollas Richard Lee",
            Email = "nicollasleeribeiro@gmail.com",
        },
        License = new OpenApiLicense
        {
            Name = "Licença XPTO 4567",
            Url = new Uri("https://example.com/license")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    else
    {
        Console.WriteLine($"Arquivo XML de documentação não encontrado em: {xmlPath}");
    }
});

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("version");
});

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PersonDtoValidator>());

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole(); // Adiciona logging para o console
});

string dbConnectionString = builder.Configuration.GetConnectionString("WindowsAuthConnection");
builder.Services.AddTransient<IDbConnection>((sp) => new SqlConnection(dbConnectionString));
DependencyInjection.Register(builder.Services);

// Configuração de autenticação JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Adicione este middleware antes do UseAuthorization
app.UseAuthorization();

app.MapControllers();
app.Run();

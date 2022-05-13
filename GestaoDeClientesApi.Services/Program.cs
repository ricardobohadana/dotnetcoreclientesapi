using GestaoDeClientesApi.Domain.Interfaces.Repositories;
using GestaoDeClientesApi.Domain.Interfaces.Services;
using GestaoDeClientesApi.Domain.Services;
using GestaoDeClientesApi.Infra.Data.Contexts;
using GestaoDeClientesApi.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);




Env.Load();
string connectionString = Environment.GetEnvironmentVariable("DBGestaoClientesAPI");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
        swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API para gestão de clientes - Projeto final do Treinamento em C# WebDeveloper da COTI Informática.",
                Description = "Projeto API desenvolvido em ASP.NET Core com .NET 6 com EntityFramework e SQL Server.",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "COTI Informática - Escola de NERDS",
                    Url = new Uri("http://www.cotiinformatica.com.br"),
                    Email = "contato@cotiinformatica.com.br"
                }
            });
        }
);

if (connectionString == null)
    connectionString = builder.Configuration.GetConnectionString("BDGestaoClientesAPI");

builder.Services.AddDbContext<SqlServerContext>(s => s.UseSqlServer(connectionString));

builder.Services.AddTransient<IClienteService, ClienteService>();
builder.Services.AddTransient<IClienteRepository, ClienteRepository>();

builder.Services.AddCors(s => s.AddPolicy("DefaultPolicy", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} else
{
    app.UseSwagger();
    app.UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/v1/swagger.json", "Clientes API"); });
}

app.UseAuthorization();

app.UseCors("DefaultPolicy");

app.MapControllers();

app.Run();

using GestaoDeClientesApi.Domain.Interfaces.Repositories;
using GestaoDeClientesApi.Domain.Interfaces.Services;
using GestaoDeClientesApi.Domain.Services;
using GestaoDeClientesApi.Infra.Data.Contexts;
using GestaoDeClientesApi.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);




Env.Load();
string connectionString = Environment.GetEnvironmentVariable("DBGestaoClientesAPI");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if(connectionString == null)
    connectionString = builder.Configuration.GetConnectionString("BDGestaoClientesAPI");

builder.Services.AddDbContext<SqlServerContext>(s => s.UseSqlServer(connectionString));

builder.Services.AddTransient<IClienteService, ClienteService>();
builder.Services.AddTransient<IClienteRepository, ClienteRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

//clienteapi
//BY9ZnWg7tgYSq5W
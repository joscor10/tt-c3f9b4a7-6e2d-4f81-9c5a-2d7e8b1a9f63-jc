using Leaderboard.Infrastructure.Context;
using Leaderboard.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration!;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
Assembly assemblyApplication = Assembly.Load("Leaderboard.Application");
builder.Services.AddMediatR(mtr =>
{
    mtr.RegisterServicesFromAssembly(assemblyApplication);
});

builder.Services.AddDbContext<PersistenceContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres")
    )
);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddPersistence(config).AddDomainServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

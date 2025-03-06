using System;
using Microsoft.EntityFrameworkCore;
using OoS.TestProject.DAL.Persistence;

var builder = WebApplication.CreateBuilder(args);

var mariaConnectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

builder.Services.AddDbContext<OoSTestProjectDbContext>(options =>
    options.UseMySql(mariaConnectionString, ServerVersion.AutoDetect(mariaConnectionString)));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

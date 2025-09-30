using Microsoft.EntityFrameworkCore;
using MiniReddit.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RedditContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();


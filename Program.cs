using Microsoft.EntityFrameworkCore;
using MiniReddit.Api.Data;
using MiniReddit.Api.Model;
using MiniReddit.Api.Service;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);


// CORS: tillad din Blazor WASM-origin (skift til din præcise origin, fx https://localhost:7225)
builder.Services.AddCors(o => o.AddPolicy("Wasm", p =>
    p.WithOrigins("https://localhost:7228")
        .AllowAnyHeader()
        .AllowAnyMethod()
));

// DbContext SQLite
builder.Services.AddDbContext<RedditContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// DI DataService
builder.Services.AddScoped<DataService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("Wasm");


static PostDto ToDto(Post p) =>
    new(p.PostId, p.Title, p.Content, p.Url, p.Author, p.Timestamp, p.Upvotes, p.Downvotes);

// === Endpoints (byt disse ud for posts) ===
app.MapGet("/api/posts", (DataService s) =>
    s.GetPosts().Select(ToDto)
);

app.MapGet("/api/posts/{id:int}", (DataService s, int id) =>
{
    var p = s.GetPost(id);
    return p is null ? Results.NotFound() : Results.Ok(ToDto(p));
});

app.MapPut("/api/posts/{id:int}/upvote", (DataService s, int id) =>
{
    if (!s.UpvotePost(id)) return Results.NotFound();
    return Results.Ok(ToDto(s.GetPost(id)!));
});

app.MapPut("/api/posts/{id:int}/downvote", (DataService s, int id) =>
{
    if (!s.DownvotePost(id)) return Results.NotFound();
    return Results.Ok(ToDto(s.GetPost(id)!));
});


app.MapPut("/api/posts/{postId:int}/comments/{commentId:int}/upvote",
    (DataService service, int postId, int commentId) =>
        service.UpvoteComment(postId, commentId) ? Results.NoContent() : Results.NotFound());

app.MapPut("/api/posts/{postId:int}/comments/{commentId:int}/downvote",
    (DataService service, int postId, int commentId) =>
        service.DownvoteComment(postId, commentId) ? Results.NoContent() : Results.NotFound());

app.MapPost("/api/posts", (DataService service, NewPostData data) =>
{
    var id = service.CreatePost(data.Title, data.Author, data.Content, data.Url);
    return new { id };
});

app.MapPost("/api/posts/{id:int}/comments", (DataService service, int id, NewCommentData data) =>
{
    var commentId = service.CreateComment(id, data.Author, data.Content);
    return commentId == 0 ? Results.NotFound() : Results.Ok(new { id = commentId });
});

app.Run();

public record PostDto(
    int Id, string Title, string? Content, string? Url,
    string Author, DateTime Timestamp, int Upvotes, int Downvotes
);
public record NewPostData(string Title, string Author, string? Content, string? Url);
public record NewCommentData(string Author, string Content);
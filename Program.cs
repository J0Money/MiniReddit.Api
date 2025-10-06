using Microsoft.EntityFrameworkCore;
using MiniReddit.Api.Data;
using MiniReddit.Api.Model;
using MiniReddit.Api.Service;

var builder = WebApplication.CreateBuilder(args);


// DbContext SQLite
builder.Services.AddDbContext<RedditContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// DI DataService
builder.Services.AddScoped<DataService>();

var app = builder.Build();

//app.UseHttpsRedirection();


app.MapGet("/api/posts", (DataService service) =>
{
    return service.GetPosts();
});


app.MapGet("/api/posts/{id:int}", (DataService service, int id) =>
{
    var post = service.GetPost(id);
    return post is null ? Results.NotFound() : Results.Ok(post);
});

app.MapPut("/api/posts/{id:int}/upvote", (DataService service, int id) =>
    service.UpvotePost(id) ? Results.NoContent() : Results.NotFound());

app.MapPut("/api/posts/{id:int}/downvote", (DataService service, int id) =>
    service.DownvotePost(id) ? Results.NoContent() : Results.NotFound());


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

public record NewPostData(string Title, string Author, string? Content, string? Url);
public record NewCommentData(string Author, string Content);
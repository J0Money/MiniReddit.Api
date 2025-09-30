namespace MiniReddit.Api.Model;

public class Post
{
    public int PostId { get; set; }
    
    public string Title { get; set; }
    
    public string Author { get; set; }
    
    public string Content { get; set; }
    
    public DateTime TimeStamp { get; set; }
    
    public List<Comment> Comments { get; set; } = new List<Comment>();
    
    public int Upvotes { get; set; }
    
    public int Downvotes { get; set; }
}
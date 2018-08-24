namespace BrightIdeas.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int PostId { get; set; }
        public Post Posts { get; set; }
    }
}
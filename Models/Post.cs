using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrightIdeas.Models
{
    public class Post
    {
        public int PostId { get; set; }
        [Required]
        [MinLength (10, ErrorMessage="Mininum of 10 characters required")]
        public string Description { get; set; }
        public List<Like> Likes { get ; set ;}
        public int UserId { get; set; }
        public User User { get ; set ;}

        public Post()
        {
            Likes = new List<Like>();
        }
    }
}
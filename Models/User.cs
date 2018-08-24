using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace BrightIdeas.Models
{
    public class User
    {
        public int UserId {get; set;}
        [Required]
        [MinLength(2)]
        public string Name {get; set;}
        [Required]
        [MinLength(2)]
        public string Alias {get; set;}
        [Required]
        [EmailAddress]
        public string Email {get; set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password is too short")]
        public string Password {get; set;}
        public List<Post> MyPosts { get ; set ;}
        public List<Like> MyLikes { get; set; }

        public User()
        {
            MyPosts = new List<Post>();
            MyLikes = new List<Like>();
            
        }
        
    }

    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email {get; set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password {get; set;}
        
    }

    public class RegisterUser
    {
        public int UserId {get; set;}
        [Required]
        [MinLength(2)]
        public string Name {get; set;}
        [Required]
        [MinLength(2)]
        public string Alias {get; set;}
        [Required]
        [EmailAddress]
        public string Email {get; set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password is too short")]
        public string Password {get; set;}
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirmation {get; set;}
        
    }
        
    
}
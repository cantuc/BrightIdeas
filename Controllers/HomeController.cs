using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BrightIdeas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace BrightIdeas.Controllers
{
    public static class SessionExtensions
    {
        // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // This helper function simply serializes theobject to JSON and stores it as a string in session
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        
        // generic type T is a stand-in indicating that we need to specify the type on retrieval
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            // Upon retrieval the object is deserialized based on the type we specified
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class HomeController : Controller
    {
        public BrightIdeasContext _context;

        public HomeController(BrightIdeasContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(User NewUser)
        {
            if(ModelState.IsValid)
            {
                
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                _context.Add(NewUser);
                _context.SaveChanges();
                HttpContext.Session.SetObjectAsJson("ActiveUser", NewUser);
                HttpContext.Session.SetInt32("ActiveUserId", NewUser.UserId);
                return RedirectToAction("BrightIdeas");
            }
            else
            {
                return View("Index");
            }
            
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginUser User)
        {
            if(ModelState.IsValid)
            {
                List<User> ActiveUser = _context.users.Where(u=>u.Email == User.Email).ToList();
                if(User != null && User.Password != null)
                {
                    var Hasher = new PasswordHasher<User>();
                    if(0 != Hasher.VerifyHashedPassword(ActiveUser[0], ActiveUser[0].Password, User.Password))
                    {
                        
                        HttpContext.Session.SetObjectAsJson("ActiveUser", ActiveUser);
                        HttpContext.Session.SetInt32("ActiveUserId", ActiveUser[0].UserId);
                        Console.WriteLine("this is ", ActiveUser);
                        ViewBag.ActiveUser = ActiveUser;
                        return RedirectToAction("BrightIdeas");
                    }
                }
            }
            return View("Login");
        }

        [HttpGet]
        [Route("brightideas")]
        public IActionResult BrightIdeas()
        {
            if(HttpContext.Session.GetInt32("ActiveUserId") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                User ActiveUser = _context.users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("ActiveUserId"));
                List<Post> AllPosts = _context.posts.Include(u=>u.User).Include(l=>l.Likes).ThenInclude(u=>u.User).ThenInclude(l=>l.MyLikes).OrderByDescending(l=>l.Likes.Count).ToList();
                ViewBag.AllPosts = AllPosts;
                ViewBag.ActiveUser = ActiveUser;
                ViewBag.UserId = (int)HttpContext.Session.GetInt32("ActiveUserId");
                return View("BrightIdeas");
            }
        }

        [HttpPost]
        [Route("addPost")]
        public IActionResult addPost(Post Posts)
        {
            int UserId = (int)HttpContext.Session.GetInt32("ActiveUserId");
            if(ModelState.IsValid)
            {
                User ActiveUser = _context.users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("ActiveUserId"));
                Post NewPost = new Post
                {
                    Description = Posts.Description,
                    UserId = (int)HttpContext.Session.GetInt32("ActiveUserId"),
                };
                _context.Add(NewPost);
                _context.SaveChanges();
                return RedirectToAction("BrightIdeas");
            }
            return View("BrightIdeas");
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }

        [Route("users/{UserId}")]
        public IActionResult SingleUser(int UserId)
        {
            User oneUser = _context.users.Include(p => p.MyPosts).Include(l => l.MyLikes).SingleOrDefault(u => u.UserId == UserId);
            Console.Write("This is the " + UserId);
            ViewBag.oneUser = oneUser;
            return View("SingleUser");
        }

        [HttpGet]
        [Route("Like/{PostId}")]
        public IActionResult Join(int PostId)
        {
            Like LikePost = new Like();
            LikePost.UserId = (int)HttpContext.Session.GetInt32("ActiveUserId");
            LikePost.PostId = PostId;
            _context.likes.Add(LikePost);
            _context.SaveChanges();
            return RedirectToAction("BrightIdeas");
        }

        [HttpGet]
        [Route("Delete/{PostId}")]
        public IActionResult Delete(int PostId)
        {
            Post DeletedPost = _context.posts.SingleOrDefault(p=>p.PostId == PostId);
            _context.posts.Remove(DeletedPost);
            _context.SaveChanges();
            return RedirectToAction("BrightIdeas");
        }

        [HttpGet]
        [Route("brightideas/{PostId}")]
        public IActionResult LikeStatus(int PostId)
        {
            Post onePost = _context.posts.Include(u=>u.User).Include(l => l.Likes).ThenInclude(u => u.User).SingleOrDefault(p=>p.PostId == PostId);
            ViewBag.onePost = onePost;
            return View("LikeStatus");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

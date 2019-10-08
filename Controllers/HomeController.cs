using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeltExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private Context db;
        public HomeController(Context context)
        {
            db = context;
        }
        // Controllers:
        [HttpGet("posts")]
        public IActionResult Index()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null)
            {
                // Fetch user and their posts.
                User GetUserById = db.Users
                .Include(u => u.Authored)
                .Include(u => u.Votes)
                    .ThenInclude(l => l.Post)
                .FirstOrDefault(u => u.UserId == (int)UserId);
                // Save user to ViewBag.
                ViewBag.User = GetUserById;
                // Fetch list of all posts.
                List<Post> AllPosts = db.Posts
                .Include(p => p.Author)
                    .ThenInclude(a => a.Votes)
                .Include(p => p.Votes)
                    .ThenInclude(l => l.User)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
                return View(AllPosts);
            }
            return RedirectToAction("LogReg", "LogReg");
        }
        [HttpGet("posts/new")]
        public IActionResult PostForm()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null)
            {
                return View();
            }
            return RedirectToAction("LogReg", "LogReg");
        }
        [HttpPost("posts/new")]
        public IActionResult NewPost(Post FormObject)
        {
            // Check if logged in.
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(ModelState.IsValid && UserId != null)
            {
                // Add Author and save to db.
                FormObject.UserId = (int)UserId;
                db.Add(FormObject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("PostForm");
        }
        [HttpGet("posts/{PostId}")]
        public IActionResult Post(int PostId)
        {
            // Check if logged in.
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null)
            {
                // Fetch user and their posts.
                User GetUserById = db.Users
                .Include(u => u.Authored)
                .Include(u => u.Votes)
                    .ThenInclude(l => l.Post)
                .FirstOrDefault(u => u.UserId == (int)UserId);
                // Save user to ViewBag.
                ViewBag.User = GetUserById;
                // Fetch Post.
                Post GetPostById = db.Posts
                .Include(p => p.Author)
                    .ThenInclude(a => a.Votes)
                .Include(p => p.Votes)
                    .ThenInclude(l => l.User)
                .FirstOrDefault(a => a.PostId == PostId);
                return View(GetPostById);
            }
            return RedirectToAction("LogReg", "LogReg");
        }
        [HttpGet("posts/{PostId}/voteup")]
        public IActionResult VoteUp(int? PostId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null && PostId != null)
            {
                // Fetch user and their posts.
                User GetUserById = db.Users
                .Include(u => u.Authored)
                .Include(u => u.Votes)
                    .ThenInclude(l => l.Post)
                .FirstOrDefault(u => u.UserId == (int)UserId);
                // Save user to ViewBag.
                ViewBag.User = GetUserById;
                // Fetch Post.
                Post GetPostById = db.Posts
                .Include(p => p.Author)
                    .ThenInclude(a => a.Votes)
                .Include(p => p.Votes)
                    .ThenInclude(l => l.User)
                .FirstOrDefault(a => a.PostId == PostId);
                // Check if user has Votes.
                foreach(var Link in GetPostById.Votes)
                {
                    if(Link.UserId == GetUserById.UserId)
                    {
                        if(Link.isUp)
                        {
                            // Do nothing if already Votes.
                            return RedirectToAction("Index");
                        }
                        if(Link.isDown)
                        {
                            // Remove Downvote.
                            db.Remove(Link);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                }
                // Create Vote(Link).
                Link NewLink = new Link();
                NewLink.UserId = (int)UserId;
                NewLink.PostId = (int)PostId;
                NewLink.isUp = true;
                db.Add(NewLink);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("LogReg", "LogReg");
        }
        [HttpGet("posts/{PostId}/Votesown")]
        public IActionResult VoteDown(int? PostId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null && PostId != null)
            {
                // Fetch user and their posts.
                User GetUserById = db.Users
                .Include(u => u.Authored)
                .Include(u => u.Votes)
                    .ThenInclude(l => l.Post)
                .FirstOrDefault(u => u.UserId == (int)UserId);
                // Save user to ViewBag.
                ViewBag.User = GetUserById;
                // Fetch Post.
                Post GetPostById = db.Posts
                .Include(p => p.Author)
                    .ThenInclude(a => a.Votes)
                .Include(p => p.Votes)
                    .ThenInclude(l => l.User)
                .FirstOrDefault(a => a.PostId == PostId);
                // Check if user has Votes.
                foreach(var Link in GetPostById.Votes)
                {
                    if(Link.UserId == GetUserById.UserId)
                    {
                        if(Link.isUp)
                        {
                            // Remove UpVote.
                            db.Remove(Link);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        if(Link.isDown)
                        {
                            // Do nothing if already Votes.
                            return RedirectToAction("Index");
                        }
                    }
                }
                // Create Vote(Link).
                Link NewLink = new Link();
                NewLink.UserId = (int)UserId;
                NewLink.PostId = (int)PostId;
                NewLink.isUp = false;
                db.Add(NewLink);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("LogReg", "LogReg");
        }
        [HttpGet("posts/{PostId}/delete")]
        public IActionResult DeletePost(int? PostId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null && PostId != null)
            {
                Post GetPostById = db.Posts.FirstOrDefault(p => p.PostId == PostId);
                // Check if User is Author.
                if(GetPostById.UserId == (int)UserId)
                {
                    db.Remove(GetPostById);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("LogReg", "LogReg");
        }
        [HttpGet("posts/{PostId}/edit")]
        public IActionResult EditPostForm(int? PostId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null && PostId != null)
            {
                Post GetPostById = db.Posts.FirstOrDefault(p => p.PostId == PostId);
                // Check if User is not Author.
                if(GetPostById.UserId != (int)UserId)
                {
                    return RedirectToAction("Index");
                }
                return View(GetPostById);
            }
            return RedirectToAction("LogReg", "LogReg");
        }
        [HttpPost("posts/{PostId}/edit")]
        public IActionResult EditPost(int? PostId, Post FormObject)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null && PostId != null)
            {
                if(ModelState.IsValid)
                {
                    Post GetPostById = db.Posts.FirstOrDefault(p => p.PostId == PostId);
                    GetPostById.Title = FormObject.Title;
                    GetPostById.Message = FormObject.Message;
                    GetPostById.UpdatedAt = DateTime.Now;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View("EditPostForm");
            }
            return RedirectToAction("LogReg", "LogReg");
        }
    }
}

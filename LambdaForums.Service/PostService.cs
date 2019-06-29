using LambdaForums.Data;
using LambdaForums.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LambdaForums.Service
{
    public class PostService : IPost
    {

        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task AddReply(PostReply reply)
        {
            _context.PostReplies.Add(reply);
            await _context.SaveChangesAsync();
        }

        //public async Task Archive(int id)
        //{
        //    var post = GetById(id);
        //    post.IsArchived = true;
        //    _context.Update(post);
        //    await _context.SaveChangesAsync();
        //}

        public Task Archive(int id)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditPostContent(int id, string newContent)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Posts
                    .Include(post => post.User)
                    .Include(post => post.Replies).ThenInclude(reply => reply.User)
                    .Include(post => post.Forum);
        }

        public Post GetById(int id)
        {
            return _context.Posts.Where(post => post.Id == id)
                   .Include(post => post.User)
                   .Include(post => post.Replies).ThenInclude(reply => reply.User)
                   .Include(post => post.Forum)
                   .First();
        }

        public IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery)
        {            
            return string.IsNullOrEmpty(searchQuery)
                ? forum.Posts 
                : forum.Posts.Where(post 
                    => post.Title.Contains(searchQuery)
                    || post.Content.Contains(searchQuery));
        }

        public IEnumerable<Post> GetFilteredPosts(string searchQuery)
        {
            var normalized = searchQuery.ToLower();

            return GetAll().Where(post
                => post.Title.ToLower().Contains(normalized)
                || post.Content.ToLower().Contains(normalized));
        }

        public IEnumerable<Post> GetLatestPost(int n)
        {
            return GetAll().OrderByDescending(post => post.Created).Take(n);
        }

        public IEnumerable<ApplicationUser> GetAllUsers(IEnumerable<Post> posts)
        {
            var users = new List<ApplicationUser>();

            foreach (var post in posts)
            {
                users.Add(post.User);

                if (!post.Replies.Any()) continue;

                users.AddRange(post.Replies.Select(reply => reply.User));
            }

            return users.Distinct();
        }


        public IEnumerable<Post> GetPostByForum(int id)
        {
            return _context.Forums
                .Where(forum => forum.Id == id).First()
                .Posts;
        }

        public IEnumerable<Post> GetPostsBetween(DateTime start, DateTime end)
        {
            return _context.Posts.Where(post => post.Created >= start && post.Created <= end);
        }

        public IEnumerable<Post> GetPostByForumId(int id)
        {
            return _context.Forums.First(forum => forum.Id == id).Posts;
        }

        public IEnumerable<Post> GetPostsByUserId(int id)
        {
            return _context.Posts.Where(post => post.User.Id == id.ToString());
        }

        public IEnumerable<Post> GetLatestPosts(int count)
        {
            var allPosts = GetAll().OrderByDescending(post => post.Created);
            return allPosts.Take(count);
        }

        public string GetForumImageUrl(int id)
        {
            var post = GetById(id);
            return post.Forum.ImageUrl;
        }        
    }
}

using SocialMedia.Data.Mongo.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Data.Mongo
{
    public interface IMongoDBWrapper
    {
        Task<string> CreatePost(DateTime workoutDate, string comment);
        Task<string> CreateComment(string comment, string postId);
        Task<Post> GetNextPost();
        Task<Comment> GetNextComment(string postId);
    }
}

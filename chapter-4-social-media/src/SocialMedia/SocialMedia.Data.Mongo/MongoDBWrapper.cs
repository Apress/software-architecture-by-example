using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SocialMedia.Data.Mongo.Entities;
using System;
using System.Threading.Tasks;

namespace SocialMedia.Data.Mongo
{
    public class MongoDBWrapper : IMongoDBWrapper
    {
        readonly IMongoDatabase _db;

        public MongoDBWrapper()
        {
            var dbClient = new MongoClient("mongodb://localhost:27017");
            _db = dbClient.GetDatabase("SocialMedia");
        }

        public async Task<string> CreateComment(string comment, string postId)
        {
            var newComment = new Comment()
            {
                PostId = postId,
                Text = comment
            };

            var collection = _db.GetCollection<Comment>("Comments");
            await collection.InsertOneAsync(newComment);

            return newComment.Id.ToString();
        }

        public async Task<string> CreatePost(DateTime workoutDate, string comment)
        {
            var newPost = new Post()
            {
                WorkoutDate = workoutDate,
                Text = comment
            };                   

            var collection = _db.GetCollection<Post>("Posts");
            await collection.InsertOneAsync(newPost);

            return newPost.Id.ToString();
        }

        public async Task<Post> GetNextPost()
        {
            var collection = _db.GetCollection<Post>("Posts");
            var result = await collection.FindOneAndDeleteAsync(a => true);
            return result;
        }

        public async Task<Comment> GetNextComment(string postId)
        {
            var collection = _db.GetCollection<Comment>("Comments");
            var result = await collection.FindOneAndDeleteAsync(a => a.PostId == postId);
            return result;
        }
    }
}


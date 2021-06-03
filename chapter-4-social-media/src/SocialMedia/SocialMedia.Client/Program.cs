using SocialMedia.Client.Helpers;
using SocialMedia.Data.Mongo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Client
{
    class Program
    {
        static readonly List<string> _posts = new List<string>();

        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Choose action");
                Console.WriteLine("1: Create Post");
                Console.WriteLine("2: Comment on Post");
                Console.WriteLine("3: Create Post and Comment");
                Console.WriteLine("4: Small Bulk Test");

                Console.WriteLine("0: Exit");

                var result = Console.ReadKey();
                switch (result.Key)
                {
                    case ConsoleKey.D1:
                        await CreatePost();
                        break;

                    case ConsoleKey.D2:
                        await CreateComment();
                        break;

                    case ConsoleKey.D3:
                        var postId = await CreateSinglePost();
                        await CreateComment(postId);
                        break;

                    case ConsoleKey.D4:
                        for (int i = 1; i <= 100; i++)
                        {
                            await CreatePost();
                            await CreateComment();
                        }
                        break;

                    case ConsoleKey.D0:
                        return;
                }
            }
        }

        private static async Task CreateComment(string postId = null)
        {
            if (postId == null) postId = _posts.GetRandom();

            var wrapper = new MongoDBWrapper();
            var createCommentResult = await wrapper.CreateComment("test comment", postId);
        }

        private static async Task<string> CreateSinglePost()
        {
            var wrapper = new MongoDBWrapper();
            var createPostResult = await wrapper.CreatePost(DateTime.Now, "test post");
            return createPostResult;
        }

        private static async Task CreatePost()
        {
            var createPostResult = await CreateSinglePost();
            _posts.Add(createPostResult);
        }
    }
}
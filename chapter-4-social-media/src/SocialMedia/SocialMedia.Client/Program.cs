using SocialMedia.Client.Helpers;
using SocialMedia.Data.Mongo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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
                Console.WriteLine("5: Large Bulk Test");

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

                    case ConsoleKey.D5:
                        for (int i = 1; i <= 100; i++)
                        {
                            Console.WriteLine($"Processing batch {i}");
                            for (int j = 1; j <= 100; j++)
                            {
                                await CreatePost();
                                await CreateComment();
                            }
                            await Task.Delay(20);
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

            var httpClient = HttpClientFactory.Create();

            var httpContent = new StringContent(postId);
            var result = await httpClient.PostAsync("https://localhost:44388/Comment", httpContent);

            Debug.Assert(result.IsSuccessStatusCode);
        }

        private static async Task<string> CreateSinglePost()
        {
            var httpClient = HttpClientFactory.Create();

            var httpContent = new StringContent("");
            var result = await httpClient.PostAsync("https://localhost:44388/Post", httpContent);

            Debug.Assert(result.IsSuccessStatusCode);

            return result.Content.ToString();            
        }

        private static async Task CreatePost()
        {
            var createPostResult = await CreateSinglePost();
            _posts.Add(createPostResult);
        }
    }
}
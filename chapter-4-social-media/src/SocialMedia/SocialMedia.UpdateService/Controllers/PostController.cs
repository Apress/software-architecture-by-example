using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialMedia.Data.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.UpdateService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;

        public PostController(ILogger<PostController> logger)
        {
            _logger = logger;
        }
      
        [HttpPost]
        public async Task<string> Create()
        {
            var wrapper = new MongoDBWrapper();
            var createPostResult = await wrapper.CreatePost(DateTime.Now, $"test post {DateTime.Now}");
            return createPostResult;
        }
    }
}

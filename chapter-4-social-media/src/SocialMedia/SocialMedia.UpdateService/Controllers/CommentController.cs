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
    public class CommentController : ControllerBase
    {        
        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task Create(string postId)
        {
            var wrapper = new MongoDBWrapper();
            var createCommentResult = await wrapper.CreateComment($"test comment {DateTime.Now}", postId);
        }

    }
}

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
        private readonly IMongoDBWrapper _mongoDBWrapper;

        public CommentController(IMongoDBWrapper mongoDBWrapper)
        {            
            _mongoDBWrapper = mongoDBWrapper;
        }

        [HttpPost]
        public async Task Create(string postId) =>        
            await _mongoDBWrapper.CreateComment($"test comment {DateTime.Now}", postId);        

    }
}

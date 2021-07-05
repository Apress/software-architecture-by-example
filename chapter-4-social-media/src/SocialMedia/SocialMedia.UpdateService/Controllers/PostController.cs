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
        private readonly IMongoDBWrapper _mongoDBWrapper;

        public PostController(IMongoDBWrapper mongoDBWrapper)
        {            
            _mongoDBWrapper = mongoDBWrapper;
        }
      
        [HttpPost]
        public async Task<string> Create() =>        
            await _mongoDBWrapper.CreatePost(DateTime.Now, $"test post {DateTime.Now}");                    
    }
}

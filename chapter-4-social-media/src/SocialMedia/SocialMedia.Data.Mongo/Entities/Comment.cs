using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Data.Mongo.Entities
{
    public class Comment : EntityBase
    {
        public string PostId { get; set; }       
        public string Text { get; set; }
    }
}

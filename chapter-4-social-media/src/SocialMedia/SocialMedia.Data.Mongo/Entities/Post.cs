using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Data.Mongo.Entities
{
    public class Post : EntityBase
    {
        public DateTimeOffset WorkoutDate { get; set; }
        public string Text { get; set; }
    }
}

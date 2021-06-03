using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Data.Mongo.Entities
{
    public class EntityBase
    {
        public ObjectId Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

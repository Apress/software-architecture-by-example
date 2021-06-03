using Dapper;
using SocialMedia.Data.Mongo;
using SocialMedia.Data.Mongo.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SocialMedia.Transfer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=SocialMedia;Integrated Security=True;");
            connection.Open();            
            
            await ReadPosts(connection, new MongoDBWrapper());            
        }

        private static async Task ReadPosts(SqlConnection connection, MongoDBWrapper wrapper)
        {
            while (true)
            {                
                // Read from Mongo                
                var nextPost = await wrapper.GetNextPost();
                if (nextPost == null) break;

                Console.WriteLine($"ReadPosts: Read {nextPost.Id}");

                using var transaction = connection.BeginTransaction();
                
                // Write To Sql Server
                string sql = "DECLARE @newRecord table(newId uniqueidentifier); "
                             + "INSERT INTO Post "
                             + "(Text, WorkoutDate) "
                             + "OUTPUT INSERTED.Id INTO @newRecord "
                             + "VALUES "
                             + "(@text, @workoutDate) "
                             + "SELECT CONVERT(nvarchar(50), newId) FROM @newRecord";
                var result = await connection.QueryAsync<string>(sql,
                    new { text = nextPost.Text, workoutDate = nextPost.WorkoutDate },
                    transaction);

                // Get all comments for post
                await ReadComments(transaction, connection, 
                    result.Single(), nextPost.Id.ToString(), wrapper);
                
                transaction.Commit();
            }

            Console.WriteLine("ReadPosts: End");
        }

        private static async Task ReadComments(SqlTransaction transaction, SqlConnection connection, 
            string postId, string filterPostId, MongoDBWrapper wrapper)
        {
            while (true)
            {                
                // Read from Mongo                
                var nextComment = await wrapper.GetNextComment(filterPostId);
                if (nextComment == null) break;

                // Write To Sql Server
                string sql = "INSERT INTO Comment "
                            + "(Text, PostId) "
                            + "VALUES "
                            + "(@text, @postId)";
                
                var result = await connection.ExecuteAsync(sql,
                    new { text = nextComment.Text, postId = postId },
                    transaction);
            }            
        }

    }
}

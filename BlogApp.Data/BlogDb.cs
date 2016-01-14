using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BlogApp.Data
{
    public class BlogDb
    {
        private readonly string _connectionString;

        public BlogDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Post> GetLastFive()
        {
            List<Post> posts = new List<Post>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 5 * From Posts ORDER By Date DESC";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    posts.Add(GetPostFromReader(reader));
                }

                return posts;
            }
        }

        public Post GetPostById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Posts WHERE ID = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                reader.Read();
                return GetPostFromReader(reader);
            }
        }

        public IEnumerable<Comment> GetCommentsForPost(int postId)
        {
            List<Comment> comments = new List<Comment>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Comments WHERE PostId = @postId";
                cmd.Parameters.AddWithValue("@postId", postId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Comment comment = new Comment
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        PostId = (int)reader["PostId"],
                        Text = (string)reader["Text"],
                        Date = (DateTime)reader["Date"]
                    };

                    comments.Add(comment);
                }

                return comments;
            }
        }

        public int AddPost(Post post)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Posts (Text, Date, Title) VALUES (@text, @date, @title); SELECT @@Identity";
                cmd.Parameters.AddWithValue("@text", post.Text);
                cmd.Parameters.AddWithValue("@date", post.Date);
                cmd.Parameters.AddWithValue("@title", post.Title);

                connection.Open();
                int id = (int)(decimal)cmd.ExecuteScalar();
                post.Id = id;
                return id;
            }
        }

        public void AddComment(Comment comment)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Comments (Name, Text, Date, PostId)" +
                                  " VALUES (@name, @text, @date, @postId)";
                cmd.Parameters.AddWithValue("@name", comment.Name);
                cmd.Parameters.AddWithValue("@text", comment.Text);
                cmd.Parameters.AddWithValue("@date", comment.Date);
                cmd.Parameters.AddWithValue("@postId", comment.PostId);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int AddTag(string name)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "AddTag";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", name);
                connection.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void AddTagsToPost(Post p, IEnumerable<Tag> tags)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO PostsTags (PostId, TagId) VALUES (@postId, @tagId)";
                connection.Open();
                foreach (Tag tag in tags)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@postId", p.Id);
                    cmd.Parameters.AddWithValue("@tagId", tag.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Tag> GetTagsForPost(int postId)
        {
            List<Tag> tags = new List<Tag>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Tags t JOIN PostsTags pt ON t.Id = pt.TagId WHERE pt.PostId = @postId";
                cmd.Parameters.AddWithValue("@postId", postId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Tag g = new Tag();
                    g.Id = (int)reader["Id"];
                    g.Name = (string)reader["Name"];
                    tags.Add(g);
                }

                return tags;
            }
        }

        private Post GetPostFromReader(SqlDataReader reader)
        {
            Post post = new Post();
            post.Id = (int)reader["Id"];
            post.Text = (string)reader["Text"];
            post.Date = (DateTime)reader["Date"];
            post.Title = (string)reader["Title"];
            return post;
        }
    }
}
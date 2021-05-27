using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Gifter.Models;
using Gifter.Utils;

namespace Gifter.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }
        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT 
                       Id, Name, Bio, Email, DateCreated, 
                       ImageUrl
                  FROM UserProfile
              ORDER BY DateCreated";

                    var reader = cmd.ExecuteReader();

                    var profiles = new List<UserProfile>();
                    while (reader.Read())
                    {
                        var profile = new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                        };
                        if (DbUtils.IsNotDbNull(reader, "Bio"))
                        {
                            profile.Bio = DbUtils.GetString(reader, "Bio");
                        };
                        if (DbUtils.IsNotDbNull(reader, "ImageUrl"))
                        {
                            profile.ImageUrl = DbUtils.GetString(reader, "ImageUrl");
                        }

                        profiles.Add(profile);
                    }

                    reader.Close();

                    return profiles;
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT 
                       Id, Name, Bio, Email, DateCreated, 
                       ImageUrl
                  FROM UserProfile
              WHERE Id = @id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    var reader = cmd.ExecuteReader();

                    UserProfile profile = null;
                    if (reader.Read())
                    {
                        profile = new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                        };
                        if (DbUtils.IsNotDbNull(reader, "Bio"))
                        {
                            profile.Bio = DbUtils.GetString(reader, "Bio");
                        };
                        if (DbUtils.IsNotDbNull(reader, "ImageUrl"))
                        {
                            profile.ImageUrl = DbUtils.GetString(reader, "ImageUrl");
                        }
                    }

                    reader.Close();

                    return profile;
                }
            }
        }

        public UserProfile GetByIdWithPosts(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                       u.Id as UserId, u.Name, u.Bio, u.Email, u.DateCreated as UserDateCreated, 
                       u.ImageUrl as UserImageUrl, p.Id as PostId, p.Title, p.ImageUrl as PostImageUrl, p.Caption, p.DateCreated as PostDateCreated
                  FROM UserProfile u
                  LEFT JOIN Post p on u.Id = p.UserProfileId
              WHERE u.Id = @id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    var reader = cmd.ExecuteReader();

                    UserProfile profile = null;
                    while (reader.Read())
                    {
                        
                        if (profile == null)
                        {
                            profile = new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "UserId"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserDateCreated"),
                                Posts = new List<Post>()
                            };
                            if (DbUtils.IsNotDbNull(reader, "Bio"))
                            {
                                profile.Bio = DbUtils.GetString(reader, "Bio");
                            };
                            if (DbUtils.IsNotDbNull(reader, "UserImageUrl"))
                            {
                                profile.ImageUrl = DbUtils.GetString(reader, "UserImageUrl");
                            }
                        }
                        if (DbUtils.IsNotDbNull(reader, "PostId"))
                        {
                            profile.Posts.Add(new Post()
                            {
                                Id = DbUtils.GetInt(reader, "PostId"),
                                Title = DbUtils.GetString(reader, "Title"),
                                ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
                                Caption = DbUtils.GetString(reader, "Caption"),
                                UserProfileId = DbUtils.GetInt(reader, "UserId"),
                                DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated")
                            });
                        }
                    }

                    reader.Close();

                    return profile;
                }
            }
        }

        public void Add(UserProfile profile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile (Name, Bio, Email, DateCreated, 
                       ImageUrl)
                        OUTPUT INSERTED.ID
                        VALUES (@name, @bio, @email, @dateCreated, @imageUrl)";

                    DbUtils.AddParameter(cmd, "@name", profile.Name);
                    DbUtils.AddParameter(cmd, "@bio", profile.Bio);
                    DbUtils.AddParameter(cmd, "@email", profile.Email);
                    DbUtils.AddParameter(cmd, "@dateCreated", DateTime.Today);
                    DbUtils.AddParameter(cmd, "@ImageUrl", profile.ImageUrl);

                    profile.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile profile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                           SET Name = @name,
                               Bio = @bio,
                               Email = @email,
                               ImageUrl = @imageUrl
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@name", profile.Name);
                    DbUtils.AddParameter(cmd, "@bio", profile.Bio);
                    DbUtils.AddParameter(cmd, "@email", profile.Email);
                    DbUtils.AddParameter(cmd, "@imageUrl", profile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Id", profile.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

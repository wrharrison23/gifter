﻿using Gifter.Models;
using System.Collections.Generic;

namespace Gifter.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        void Delete(int id);
        List<Post> GetAll();
        Post GetById(int id);
        void Update(Post post);
        public List<Post> GetAllWithComments();
        public Post GetByIdWithComments(int id);
    }
}
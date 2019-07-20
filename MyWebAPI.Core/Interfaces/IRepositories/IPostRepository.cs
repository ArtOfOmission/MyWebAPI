using MyWebAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyWebAPI.Core.Interfaces.IRepositories
{
    public interface IPostRepository
    {
       Task<IEnumerable<Post>> GetAllPostsAsync();
        void AddPost(Post post);

    }
}

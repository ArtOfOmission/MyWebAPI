using MyWebAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyWebAPI.Core.Interfaces.IRepositories
{
    /// <summary>
    /// 文章仓储接口
    /// </summary>
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int id);
        void AddPost(Post post);
    }
}

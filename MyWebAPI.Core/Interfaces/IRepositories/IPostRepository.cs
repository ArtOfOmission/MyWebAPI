using MyWebAPI.Core.Entities;
using MyWebAPI.Core.EntityParameters;
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
        Task<PaginatedList<Post>> GetAllPostsAsync(PostParameter postParameters);
        Task<Post> GetPostByIdAsync(int id);
        void AddPost(Post post);
        void Deletet(Post post);
        void Update(Post post);
    }
}

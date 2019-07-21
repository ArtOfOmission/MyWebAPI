using Microsoft.EntityFrameworkCore;
using MyWebAPI.Core.Entities;
using MyWebAPI.Core.EntityParameters;
using MyWebAPI.Core.Interfaces.IRepositories;
using MyWebAPI.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebAPI.Infrastructure.Repositories
{
    /// <summary>
    /// 文章仓储
    /// </summary>
    public class PostRepository : IPostRepository
    {
        private readonly MyContext _myContext;

        public PostRepository(MyContext myContext)
        {
            this._myContext = myContext;
        }

        /// <summary>
        /// 获取所有文章
        /// </summary>
        /// <returns></returns>
        public async Task<PaginatedList<Post>> GetAllPostsAsync(PostParameter postParameters)
        {
            var query = this._myContext.Posts.OrderBy(x => x.Id);

            var count = await query.CountAsync();

            var list = await query
                .Skip((postParameters.PageIndex - 1) * postParameters.PageSize)
                .Take(postParameters.PageSize)
                .ToListAsync();

            return new PaginatedList<Post>(postParameters.PageIndex, postParameters.PageSize, count, list); 

        }

        /// <summary>
        /// 根据id获取文章
        /// </summary>
        /// <returns></returns>
        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await this._myContext.Posts.FindAsync(id);
        }

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="post"></param>
        public void AddPost(Post post) {
            this._myContext.Posts.Add(post);
        }


    }
}

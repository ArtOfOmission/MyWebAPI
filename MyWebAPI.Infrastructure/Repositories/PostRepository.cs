using Microsoft.EntityFrameworkCore;
using MyWebAPI.Core.Entities;
using MyWebAPI.Core.Interfaces.IRepositories;
using MyWebAPI.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebAPI.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly MyContext _myContext;

        public PostRepository(MyContext myContext)
        {
            this._myContext = myContext;
        }

        /// <summary>
        /// 获取所有推文
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await this._myContext.Posts.ToListAsync();
        }

        /// <summary>
        /// 添加推文
        /// </summary>
        /// <param name="post"></param>
        public void AddPost(Post post) {
            this._myContext.Posts.Add(post);
        }
    }
}
